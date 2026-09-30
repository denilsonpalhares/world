# Roadmap

Regra: primeiro a simulação funcionando sem gráficos, depois a parte visual. Cada fase termina com algo testável.

## Fase 0 — Ambiente
- [x] Godot 4.7.2 .NET instalado (`godot`)
- [x] .NET SDK 10 instalado
- [x] `git init` + `.gitignore` + remoto GitHub
- [x] Solução com `Simulation`, `Simulation.Tests`, `tools/Headless`
- [x] Projeto Godot em `/game` referenciando `Simulation`
- ✅ Pronto quando: `dotnet build` e `dotnet test` passam

## Fase 1 — Esqueleto da simulação
- [x] Decisões de estrutura do mundo ([docs/WORLD_DESIGN.md](docs/WORLD_DESIGN.md))
- [x] `WorldState` (estado, RNG com seed, contador de ticks, calendário)
- [x] Laço de tick com pipeline de sistemas em ordem fixa
- [x] Fila de comandos (`ICommand`) processada no início do tick
- [x] Carregador de dados JSON de `/data`
- [x] Teste de determinismo
- ✅ Pronto quando: teste de determinismo passa

## Fase 2 — Mundo mínimo
- [ ] Definir conteúdo físico: relevo, biomas, recursos, rios (seção 5 do WORLD_DESIGN)
- [ ] Grafo do mapa: províncias terrestres, zonas marítimas, adjacências, costas/portos, ilhas
- [ ] Províncias (terreno, recursos, dono, vizinhos) e Estados
- [ ] Nações (estados/províncias, capital, tesouro, leis ativas)
- [ ] Pops (tamanho, profissão, cultura, escolaridade, renda, felicidade)
- [ ] Gerador de mundo inicial
- ✅ Pronto quando: o headless imprime um resumo do mundo

## Fase 3 — Leis e modificadores
- [ ] Formato das leis em JSON + agregação de modificadores por nação
- [ ] 5 a 10 leis iniciais
- ✅ Pronto quando: aprovar uma lei altera os modificadores da nação (testado)

## Fase 4 — Sistemas dos cidadãos
- [ ] Demografia
- [ ] Educação
- [ ] Produção
- [ ] Mercado e comércio
- [ ] Governo e orçamento
- [ ] Felicidade e estabilidade
- [ ] Migração
- ✅ Pronto quando: 100 anos simulados com estatísticas plausíveis

## Fase 5 — Ferramenta de balanceamento
- [ ] Headless exporta estatísticas em CSV
- [ ] Ajuste dos números em `/data`

## Fase 6 — Visualização no Godot (máquina com tela)
- [ ] Mapa de regiões colorido por nação
- [ ] Controle de tempo (pausa, 1x/2x/5x)
- [ ] Painéis de nação, região e pops

## Fase 7 — O presidente (primeiro protótipo jogável)
- [ ] Tela de leis
- [ ] Tela de orçamento
- [ ] Notificações de eventos emergentes

## Fase 8 — Nações de IA
- [ ] IA emite os mesmos comandos que o jogador

## Fase 9 — Relações entre nações
- [ ] Diplomacia
- [ ] Guerra e ocupação
- [ ] Refugiados

## Fase 10 — Persistência e acabamento
- [ ] Salvar e carregar
- [ ] Localização pt-BR
- [ ] Tutorial, sons e polimento
