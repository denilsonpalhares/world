# CLAUDE.md — World (Simulador de Nações)

## Visão do jogo

Simulador de mundo / grand strategy. O mundo **sempre** contém diversas nações, cada uma com leis próprias.
Com base nessas leis, os cidadãos crescem, estudam, trabalham, produzem, comerciam, constroem, migram/fogem,
se revoltam e invadem. O jogador é o **presidente de uma nação**: não controla indivíduos, apenas define
**direcionamentos** (leis, orçamento, diplomacia, prioridades). O comportamento do mundo deve **emergir** da
simulação, nunca ser roteirizado.

## Stack

- **Engine:** Godot 4.7 (versão .NET), comando `godot`
- **Linguagem:** C# (.NET 10) em todo o projeto. Não usar GDScript, exceto em ferramentas de editor triviais.
- **Dados:** JSON para definições de conteúdo (leis, recursos, profissões, culturas, nações iniciais).
- **Testes:** xUnit no projeto `Simulation.Tests`.

## Arquitetura (regra mais importante)

```
/game            → projeto Godot (apresentação: mapa, UI, input, áudio)
/src/Simulation  → biblioteca C# pura (núcleo da simulação) — SEM referência ao Godot
/src/Simulation.Tests → testes xUnit do núcleo
/data            → definições de conteúdo em JSON (moddable)
/tools           → scripts utilitários (ex.: rodar simulação headless por N anos)
```

1. **`Simulation` nunca referencia `Godot` / `GodotSharp`.** Nenhum `Node`, `Vector2` do Godot, `GD.Print` etc.
   Deve compilar e rodar sozinha em um console app.
2. **A camada Godot apenas lê o estado e envia comandos.** Toda mudança no mundo passa por um `ICommand`
   (ex.: `EnactLawCommand`, `SetBudgetCommand`, `DeclareWarCommand`) processado no próximo tick.
   A UI nunca altera o estado da simulação diretamente.
3. **Comunicação sim → UI por eventos/snapshots** (ex.: `WorldEvents`, relatórios por tick), não por polling
   de estruturas internas mutáveis.

## Regras da simulação

- **Tick determinístico.** A simulação avança em ticks discretos (1 tick = 1 dia; sistemas pesados podem rodar
  mensalmente). Mesmo estado inicial + mesma seed + mesmos comandos = mesmo resultado, sempre.
  - Usar um único RNG com seed controlada pelo `World` (nunca `new Random()` solto, nunca `DateTime.Now`).
  - Iteração em ordem estável (evitar depender da ordem de `Dictionary`/`HashSet`; usar listas ou chaves ordenadas).
  - Evitar `float` em lógica que precise ser reproduzível entre máquinas quando for crítico; preferir `double`
    consistente ou ponto fixo onde necessário.
- **População em "Pops", não indivíduos.** Um `Pop` é um grupo homogêneo (região, profissão, cultura, religião,
  escolaridade, renda, felicidade, tamanho). Pops crescem, encolhem, se dividem (ex.: parte estuda e muda de
  profissão) e migram. Indivíduos detalhados só para personagens notáveis (líderes, generais), se/quando existirem.
- **Leis são dados, não código.** Cada lei é definida em `/data/laws/*.json` como um conjunto de **modificadores**
  (ex.: `education_rate +0.2`, `budget_expense +0.05`, `happiness[lower_class] +3`). Os sistemas leem
  modificadores agregados; não criar `if (law == "X")` espalhado pelo código.
- **Sistemas separados e ordenados.** Cada aspecto é um sistema com responsabilidade única, executado em ordem
  fixa por tick: Demografia → Educação → Produção → Mercado/Comércio → Governo/Orçamento → Felicidade/Estabilidade
  → Migração → Diplomacia → Guerra. Adicionar novos sistemas no pipeline em vez de inchar os existentes.
- **Comportamento emergente.** Pops tomam decisões por utilidade/probabilidade com base nas condições locais e nas
  leis (ex.: migrar se a qualidade de vida em outra região for maior o suficiente). Nada de eventos roteirizados
  para produzir resultados que a simulação deveria gerar.
- **Nações de IA usam as mesmas regras e comandos que o jogador.** A IA emite `ICommand`s como o presidente;
  nenhuma trapaça no núcleo.
- **Save/Load** = serializar o estado do `World` (+ seed/RNG state). Estado deve ser serializável desde o início.

## Performance

- O mundo deve suportar centenas de regiões e milhares de pops rodando rápido em modo headless.
- Evitar alocações por tick em loops quentes (LINQ em hot paths, boxing, closures). Preferir arrays/`List<T>`
  e structs quando fizer diferença medida.
- Otimizar só com medição (benchmark headless em `/tools`).

## Convenções de código

- C# moderno: `nullable enable`, `file-scoped namespaces`, `record` para dados imutáveis/comandos.
- Nomes de código (classes, métodos, variáveis, IDs de dados) em **inglês**. Textos exibidos ao jogador via
  arquivos de localização (pt-BR como idioma principal).
- Namespaces: `World.Simulation.*` para o núcleo, `World.Game.*` para a camada Godot.
- Nós Godot: scripts C# pequenos, apenas apresentação/binding. Lógica de jogo vai para `Simulation`.
- Sem números mágicos de balanceamento no código: constantes de balanceamento vão para `/data`.

## Comandos

```bash
dotnet build World.sln                                  # compila tudo (inclui o projeto Godot)
dotnet test World.sln                                   # roda os testes
dotnet run --project tools/Headless                     # simulação sem gráficos
godot --headless --path game --build-solutions --quit   # compila pelo Godot
godot --headless --path game --quit-after 2             # roda a cena principal sem tela
```

- `World.sln` (raiz) é a solução principal. `game/Game.sln` existe só porque o editor Godot a procura.
- O projeto Godot é `game/Game.csproj` (`Godot.NET.Sdk/4.7.2`, `net10.0`) e referencia `src/Simulation`.
- Arquivos `*.uid` gerados pelo Godot devem ser versionados.

## Testes e verificação

- Toda regra de simulação nova deve ter teste em `Simulation.Tests`.
- Manter um teste de **determinismo**: rodar N ticks duas vezes com a mesma seed e comparar hash do estado.
- Antes de concluir uma mudança no núcleo: `dotnet build` e `dotnet test` devem passar.
- Para balanceamento, usar a simulação headless (`/tools`) rodando décadas/séculos e inspecionando estatísticas.

## Fluxo de trabalho

- Ambiente: o desenvolvimento do núcleo roda num **Ubuntu sem interface gráfica** (Godot só com `--headless`).
  O editor Godot e os testes visuais rodam numa máquina **Windows**. A sincronização é feita pelo git
  (`git@github.com:denilsonpalhares/world.git`, branch `main`).
- Etapas e progresso: [ROADMAP.md](ROADMAP.md). Marcar os itens concluídos.
- Construir incrementalmente: primeiro o núcleo headless funcionando e testado, depois a visualização no Godot.
- Mudanças de arquitetura (novos sistemas, formato de dados, contrato de comandos) devem ser discutidas antes.
- Responder e documentar em português (pt-BR).
