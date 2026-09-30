# Design do Mundo

Documento de decisões sobre o mundo físico e a estrutura territorial.
Status: **Decidido** = não mudar sem discussão · **Aberto** = a definir na fase indicada.

## 1. Escala e formato — Decidido

- **Mapa de províncias**: polígonos irregulares; cada província é a **menor unidade territorial** (equivale a um
  município: uma cidade e seus arredores).
- **Escala "continente"**: um continente principal, **mares** e **ilhas**.
  - ~500 a 1.500 províncias terrestres.
  - 5 a 15 nações no início (pode haver territórios sem dono para colonizar).
- **Hierarquia territorial**:

```
Nação
 └── Estado (agrupamento administrativo de províncias; permite leis regionais e governadores)
      └── Província (unidade básica; tem terreno, recursos, construções, pops e um dono)
           └── Pops (grupos homogêneos de cidadãos)
```

- Províncias **mudam de dono** por conquista, tratado/venda, secessão, colonização ou unificação.
  Os pops permanecem na província e passam a seguir as leis do novo dono.
- Estados pertencem a uma nação. Ao mudar de dono, a província sai do estado antigo e entra num estado da nação
  nova (existente ou criado).

## 2. Mar e ilhas — Decidido

- O mar é dividido em **zonas marítimas**: são nós do mesmo grafo de adjacência das províncias, mas **não têm
  pops nem dono** (podem ter recursos, como pesca).
- Tipos de nó do mapa: `Land` (província), `Sea` (zona marítima costeira/aberta), `Lake` (opcional).
- **Províncias costeiras** são vizinhas de pelo menos uma zona marítima e podem ter **porto**.
- **Ilhas** são províncias terrestres cujas únicas conexões com o resto do mundo passam por zonas marítimas.
  Migração, comércio e invasão para/de ilhas exigem rota marítima (porto + transporte naval).
- Movimento, comércio e migração usam o **mesmo grafo** (caminhos por terra e por mar, com custos diferentes).

## 3. Tempo — Decidido

- **1 tick = 1 dia**. O calendário tem meses e anos; os sistemas rodam com frequência própria
  (diária, mensal ou anual) conforme o custo e a natureza de cada um.
- **Estações do ano** existem e influenciam produção agrícola, clima e deslocamentos.

## 4. Geração do mundo — Decidido

- **Procedural com seed**: cada partida pode ter um mundo novo; mesma seed = mesmo mundo.
- **Mapas fixos** também podem ser carregados de `/data/maps/` (para testes e cenários).
- Em ambos os casos o resultado é o mesmo formato de dados (grafo de nós + atributos), então a simulação não
  sabe de onde o mapa veio.

## 5. Conteúdo físico — Aberto (Fase 2)

Definido em `/data`, não em código:
- **Relevo**: planície, colina, montanha, vale, costa (afeta construção, defesa, transporte).
- **Bioma / vegetação**: mata, floresta, campo, deserto, tundra, pântano (afeta agricultura, madeira, saúde).
- **Recursos naturais**: renováveis (terra fértil, madeira, peixe, água) e finitos (ferro, carvão, ouro,
  petróleo...), com esgotamento.
- **Hidrografia**: rios (fertilidade, comércio, fronteiras naturais) e lagos.

## 6. Mundo dinâmico — Aberto (Fase 4+)

- **Clima**: temperatura e chuva por estação; anos bons e ruins de colheita.
- **Eventos naturais**: seca, enchente, inverno rigoroso (geram fome e migração).
- **Impacto humano**: desmatamento, esgotamento de minas, degradação do solo.
