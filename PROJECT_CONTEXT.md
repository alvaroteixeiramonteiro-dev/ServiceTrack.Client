# ServiceTrack — Contexto do Projeto

> Documento de conhecimento permanente do Project **ServiceTrack** no Claude.
> Data da análise: **10-11-2026**.
> Referências analisadas: **ServiceTrack.API `1e88578`** e **ServiceTrack.Client `48b00c9`** (ramo `main` em ambos).
> Se os repositórios tiverem commits posteriores, este documento pode estar desatualizado. Ver secção 10.

## Legenda das fontes

- **[CÓDIGO]**: lido diretamente no código dos repositórios, nos commits de referência.
- **[README]**: afirmação dos READMEs dos repositórios. Só é tratada como confirmada quando validada contra o código.
- **[INFERIDO]**: deduzido a partir do código, mas não executado nem observado.
- **[NÃO VERIFICADO]**: não foi possível confirmar.

## Método e limitações da análise

- O **conetor GitHub não estava disponível** nesta sessão (não surgiu nas ferramentas e o `web_fetch` às páginas do GitHub foi recusado).
- A análise foi feita por **`git clone`/`git fetch` só de leitura dos repositórios públicos**, sem alterar ficheiros, sem commits e sem escrever código.
- **Nada foi compilado nem executado.** Não houve build, arranque da API, ligação a MySQL, nem execução do cliente WinForms.
- **Nenhum teste foi realizado.** Os cenários de "verificação manual" descritos nos READMEs são afirmações do autor e **não foram reproduzidos nem confirmados**.
- Não foram inspecionados issues, pull requests, Actions, repositórios privados, nem a lista completa de repositórios da conta (a tentativa de listagem pela API pública falhou por limite de pedidos).

---

## 1. Objetivo e âmbito

- ServiceTrack é uma aplicação de **gestão de pedidos de serviço**: registar, consultar, editar e eliminar pedidos, com prioridade, estado e datas de criação, atualização e resolução.
- Os READMEs descrevem o projeto como desenvolvido no âmbito da **UC00614 — Integrar Sistemas de Informação**, numa **versão educativa/de portefólio** com **uma única tabela** e sem camadas arquiteturais adicionais.
- **Fora do âmbito atual** [README, confirmado no código]: autenticação e autorização, múltiplas tabelas, utilizadores, ligação direta do cliente à base de dados.

## 2. Arquitetura e relação entre os repositórios

Dois repositórios independentes, duas soluções (`.slnx`):

```
ServiceTrack.Client (WinForms)  --HTTPS/JSON-->  ServiceTrack.API (ASP.NET Core)  --MySqlConnector/SQL-->  MySQL (servicetrack)
```

- **ServiceTrack.API**: Web API com controladores. O acesso a dados é feito **diretamente no controlador** com ADO.NET (`MySqlConnection`/`MySqlCommand`) e SQL parametrizado. Não há EF Core, repositórios, serviços nem DTOs [CÓDIGO].
- **ServiceTrack.Client**: aplicação Windows Forms com janela MDI. Comunica com a API por `HttpClient`. **Não se liga ao MySQL** [CÓDIGO].
- Contrato partilhado: não existe projeto/biblioteca comum. A classe `Pedido` está **duplicada** em cada repositório [CÓDIGO].
- Endereço da API usado pelo cliente: `https://localhost:7242/` (valor fixo em `Services/ApiService.cs`).
- O cliente depende de o certificado HTTPS de desenvolvimento da API ser confiável na máquina [README].

## 3. Tecnologias e versões identificadas

### API [CÓDIGO]

| Item | Valor |
|---|---|
| Framework | `net10.0` (SDK Web) |
| Nullable / ImplicitUsings | ativos |
| Pacotes NuGet | `MySqlConnector` 2.6.2; `Swashbuckle.AspNetCore` 10.3.1 |
| Base de dados | MySQL (`servicetrack`, `utf8mb4_unicode_ci`) |
| Swagger | ativo apenas em Development |
| URLs de desenvolvimento | `https://localhost:7242` e `http://localhost:5039` |
| Solução | `ServiceTrack.API.slnx` |

### Cliente [CÓDIGO]

| Item | Valor |
|---|---|
| Framework | **.NET Framework 4.7.2** (não é .NET 10) |
| Tipo de projeto | `.csproj` antigo (não SDK-style), com `packages.config` |
| UI | Windows Forms, MDI |
| Pacotes | `System.Net.Http.Json` 10.0.14, `System.Text.Json` 10.0.14 e dependências associadas (via NuGet) |
| Solução | `ServiceTrack.Client.slnx` |
| Versão de C# | Sem `LangVersion` explícita; o padrão para net472 é C# 7.3 [INFERIDO]. Funcionalidades mais recentes (por exemplo `using var`) não compilam no cliente. |

> A memória importada e algumas descrições iniciais sugerem .NET 10 em todo o projeto. **Isso só é verdade para a API.**

## 4. Base de dados e operações CRUD

### Esquema [CÓDIGO: `database/servicetrack.sql`]

Uma única tabela, `pedidos`:

| Coluna | Tipo | Notas |
|---|---|---|
| `id` | `INT` | PK, `AUTO_INCREMENT` |
| `titulo` | `VARCHAR(255)` | `NOT NULL` |
| `descricao` | `TEXT` | `NOT NULL` |
| `nome_contato` | `VARCHAR(255)` | `NOT NULL` |
| `email_contato` | `VARCHAR(255)` | `NOT NULL` |
| `categoria` | `VARCHAR(50)` | `NOT NULL` (texto livre) |
| `prioridade` | `VARCHAR(20)` | `NOT NULL`, default `'Normal'` |
| `estado` | `VARCHAR(20)` | `NOT NULL`, default `'Aberto'` |
| `criado_em` | `DATETIME` | `NOT NULL` |
| `atualizado_em` | `DATETIME` | `NOT NULL` |
| `resolvido_em` | `DATETIME` | `NULL` |

- Não existem índices além da PK, restrições `CHECK`/`ENUM` (prioridade e estado só são garantidos pela API), tabelas auxiliares, dados de exemplo nem migrações.
- O script usa `CREATE ... IF NOT EXISTS`.
- Ligação: `ConnectionStrings:DefaultConnection` em `appsettings.json`, configurada com utilizador `root` e sem palavra-passe. O README recomenda User Secrets se houver palavra-passe.

### Endpoints [CÓDIGO: `Controllers/PedidosController.cs`]

Rota base `api/[controller]` → `/api/Pedidos`.

| Método | Rota | Sucesso | Erros |
|---|---|---|---|
| GET | `/api/Pedidos` | 200, lista ordenada por `criado_em DESC, id DESC` | |
| GET | `/api/Pedidos/{id:int}` | 200 | 404 `{ mensagem }` |
| POST | `/api/Pedidos` | 201 (`CreatedAtAction`) | 400 `{ mensagem }` |
| PUT | `/api/Pedidos/{id:int}` | 204 | 400, 404 |
| DELETE | `/api/Pedidos/{id:int}` | 204 | 404 |

### Modelo `Pedido`

`Id`, `Titulo`, `Descricao`, `NomeContato`, `EmailContato`, `Categoria`, `Prioridade` (default `"Normal"`), `Estado` (default `"Aberto"`), `CriadoEm`, `AtualizadoEm`, `ResolvidoEm?`.

### Validações (POST e PUT, repetidas nos dois) [CÓDIGO]

- Obrigatórios (`IsNullOrWhiteSpace`): título, descrição, nome do contacto, email, categoria.
- Comprimentos máximos: título, nome e email 255; categoria 50. A descrição não tem limite verificado.
- `Prioridade` ∈ {`Baixa`, `Normal`, `Alta`}.
- `Estado` ∈ {`Aberto`, `Em curso`, `Resolvido`}.
- O email só é validado quanto a presença e tamanho, **não quanto ao formato**.

### Regras de datas [CÓDIGO]

- POST: `CriadoEm = AtualizadoEm = DateTime.UtcNow`. `ResolvidoEm` é preenchido se o estado for `Resolvido`.
- PUT: `AtualizadoEm = DateTime.UtcNow`. Se o estado for `Resolvido`, `ResolvidoEm = AtualizadoEm`; caso contrário fica `NULL`. O UPDATE SQL não altera `criado_em`.

## 5. Funcionalidades implementadas

### API [CÓDIGO]

- CRUD completo de pedidos (5 endpoints acima).
- Validação de campos, comprimentos, prioridade e estado.
- Gestão de datas de criação, atualização e resolução pela API.
- Consultas SQL parametrizadas.
- Swagger/Swagger UI em Development.
- Script SQL de criação da base de dados e da tabela.
- Ficheiro `ServiceTrack.API.http` com dois pedidos de exemplo (GET lista e GET por id).
- README com requisitos, configuração, endpoints e âmbito.

### Cliente [CÓDIGO]

- Janela principal MDI (`principal`), com menu **Pedidos → Gerir pedidos** que abre `FormPedidos` como filho.
- `FormPedidos` com:
  - `DataGridView` (`dgvPedidos`) só de leitura, seleção de linha completa, sem multisseleção;
  - campos de texto para título, descrição (multilinha), nome do contacto, email e categoria, com `MaxLength` 255/255/255/50;
  - combos de prioridade e estado (`DropDownList`) com os valores fixos;
  - botões **Limpar**, **Guardar**, **Atualizar**, **Eliminar**, **Recarregar**.
- Comportamento:
  - carrega a lista ao abrir o formulário;
  - clicar numa linha preenche os campos e guarda o id selecionado;
  - **Guardar** faz sempre POST; **Atualizar** exige seleção e faz PUT; **Eliminar** pede confirmação e faz DELETE;
  - após sucesso, mostra mensagem, limpa os campos e recarrega a grelha;
  - verificação de campos obrigatórios antes de chamar a API;
  - mensagens de erro de comunicação (`try/catch`).
- `ApiService` estático com `HttpClient`, métodos GET/POST/PUT/DELETE.
- Layout com ancoragens (campos Top|Left|Right; grelha nas quatro direções; botões Bottom|Left|Right). Segundo a descrição fornecida pelo utilizador, a correção visual de scroll e botões cortados foi confirmada por ele. No Designer de `FormPedidos` não existem `AutoScroll` nem `MinimumSize`.

### Estado face ao que estava "por confirmar" no documento inicial do Project

| Item | Estado |
|---|---|
| Integração de POST/PUT/DELETE no cliente | **Implementada** [CÓDIGO] |
| Tratamento de erros | **Parcial** (ver problemas 1 e 4) |
| Testes | **Inexistentes** |
| Acabamento da UI | **Parcial** (problemas 8, 9 e 10) |

## 6. Problemas técnicos conhecidos

### 6.1 Confirmados no código (nos commits de referência)

| # | Problema | Onde |
|---|---|---|
| 1 | O cliente **descarta as mensagens de erro da API** (`{ mensagem }`). O `ApiService` devolve só `bool` e a UI mostra textos genéricos. | `Services/ApiService.cs` (POST/PUT/DELETE); `FormPedidos.cs` (blocos `else` de guardar/atualizar/eliminar) |
| 2 | Os botões **não são desativados durante o pedido**; um duplo clique em Guardar pode enviar dois POST. | `FormPedidos.cs`, `btnGuardar_Click` (`async void`) |
| 3 | O PUT **refaz `ResolvidoEm`** sempre que se atualiza um pedido já resolvido, perdendo a data original. O README descreve-o como comportamento intencional. | `PedidosController.cs`, método `Atualizar` |
| 4 | **Sem tratamento global de exceções** na API; os controladores não têm `try/catch`. Com a BD indisponível, a resposta é um 500 não controlado. | `Program.cs`; `PedidosController.cs` |
| 5 | Validação **duplicada** em POST e PUT e mapeamento de `Pedido` duplicado nos dois GET. O modelo `Pedido` serve de corpo do pedido (sem DTOs); o cliente pode enviar `Id` e datas, que são ignorados ou sobrescritos. | `PedidosController.cs` |
| 6 | Email **sem validação de formato** na API e no cliente. Categoria é texto livre. | API e `FormPedidos.cs`; README |
| 7 | Configuração versionada usa `root` **sem palavra-passe**. Não expõe segredos, mas é má prática. | `appsettings.json` |
| 8 | Grelha com **colunas autogeradas** (não há configuração de colunas): cabeçalhos com nomes das propriedades, descrição completa, datas sem formato. | `FormPedidos.Designer.cs` / `FormPedidos.cs` |
| 9 | **Resíduos do template MDI**: `Form1` vazio e não usado (ainda compilado); "Nova"/"Nova Janela" criam janelas em branco; "Abrir"/"Salvar como" abrem diálogos sem efeito; vários itens de menu sem handler (Imprimir, Desfazer, Refazer, Opções, Ajuda…); handlers vazios (`label*_Click`, `dataGridView1_CellContentClick`, `txtDescricao_TextChanged`, `toolStripStatusLabel_Click`, Cortar/Copiar/Colar). | `Form1.*`, `principal.cs`, `principal.Designer.cs`, `FormPedidos.cs` |
| 10 | Textos **mistos pt-PT/pt-BR** e gralhas ("Salvar", "Arquivos de texto", "Exibir", "Barra de Status", "Vizualizar impressão", "Sobre ... ..."). A classe da janela principal chama-se `principal` (minúsculas). | `principal.Designer.cs`, `principal.cs` |
| 11 | A janela principal está definida com `WindowState = Maximized`, o que **contraria a regra do projeto de não impor maximização**. | `principal.Designer.cs` |
| 12 | `ServiceTrack.API.http` só contém GET lista e GET por id. | `ServiceTrack.API.http` |
| 13 | **Sem testes automáticos** em nenhum dos repositórios e sem CI. | árvore de ficheiros |
| 14 | O endereço da API está **fixo no código** do cliente (documentado no README). | `Services/ApiService.cs` |

### 6.2 Possíveis problemas inferidos (não executados, a confirmar)

| # | Possível problema | Raciocínio |
|---|---|---|
| A | **Fuso horário/UTC:** a API grava `UtcNow` em `DATETIME`; ao ler, `GetDateTime` devolve `Kind` não especificado, o JSON sai sem indicador de UTC e o cliente tende a mostrar a hora UTC como se fosse local. | Comportamento típico de `DATETIME` + MySqlConnector + System.Text.Json. |
| B | Selecionar linhas **com as setas do teclado** não atualiza os campos, porque só se usa `CellClick`. | Só existe handler de `CellClick`. |
| C | Ao reduzir a janela abaixo do tamanho de desenho, os controlos ficam cortados; os botões (Left|Right) esticam ao alargar. | Sem `AutoScroll`/`MinimumSize` e com ancoragens no Designer. |
| D | Sem timeout configurado no `HttpClient`, o cliente pode ficar bloqueado durante muito tempo se a API não responder. | Usa o valor por omissão do `HttpClient`. |
| E | Os pedidos em `async void` sem proteção de reentrada podem produzir estados inconsistentes na UI em ações rápidas e repetidas. | Relacionado com o problema 2. |

## 7. Funcionalidades pendentes e melhorias recomendadas

**Pendente (ausente do código):**
- Testes automáticos (API e cliente).
- Validação do formato do email.
- Autenticação/autorização (fora do âmbito declarado).
- Paginação, pesquisa, filtros e ordenação na listagem e na grelha.
- Tratamento global de erros na API com resposta de erro uniforme.
- Limpeza do template MDI no cliente.

**Melhorias recomendadas:**
- Extrair a validação duplicada da API para um único ponto; considerar DTOs de entrada.
- Categorias controladas (lista fixa ou tabela) em vez de texto livre.
- Restrições na BD (`CHECK`/`ENUM`) e índices adequados, se o volume o justificar.
- Utilizador MySQL dedicado e User Secrets para a ligação.
- Completar `ServiceTrack.API.http` com POST, PUT e DELETE.
- Configuração do endereço da API fora do código (por exemplo, `App.config`).
- Decisão futura, opcional e de custo elevado: migrar o cliente para .NET moderno (SDK-style).

## 8. Próximas tarefas por prioridade

Princípios: uma funcionalidade por commit, teste rápido a cada passo e README atualizado. A tarefa 1 está recomendada como primeira.

| Prio | Tarefa | Repositório | Critério de conclusão |
|---|---|---|---|
| 1 | Mostrar no cliente as **mensagens de erro da API**. Alterar o contrato do `ApiService` (resultado além de `bool`) e o tratamento na `FormPedidos`. | Client | Um POST/PUT inválido mostra a mensagem devolvida pela API. |
| 2 | **Evitar submissões duplicadas**: desativar botões durante cada pedido e repor no fim. | Client | Duplo clique em Guardar cria um só pedido. |
| 3 | **Datas e grelha**: decidir o tratamento de UTC (converter no cliente ou serializar UTC explícito na API); configurar colunas (cabeçalhos em português, formato de data, descrição truncada ou oculta). | Client (e API, conforme a decisão) | Datas coerentes com a hora local; grelha legível. |
| 4 | **Regra de `ResolvidoEm`**: decidir se mantém a data original quando o pedido já estava resolvido; atualizar API e README. | API | Comportamento definido, documentado e testado manualmente. |
| 5 | **Validar o formato do email** na API e no cliente, de forma consistente. | API + Client | Email inválido rejeitado com mensagem clara. |
| 6 | **Robustez da API**: tratamento global de exceções, utilizador MySQL dedicado em vez de `root`, User Secrets. | API | BD indisponível devolve erro controlado; sem credenciais no repositório. |
| 7 | **Limpeza do cliente**: remover `Form1` e itens/handlers do template sem função, uniformizar para pt-PT, rever `WindowState`. Um commit por passo (edições no Designer são frágeis). | Client | O formulário abre e funciona após cada passo; menus só com itens úteis. |
| 8 | **Qualidade**: extrair a validação duplicada da API, adicionar pequeno projeto de testes sobre essa lógica, completar o `.http`. | API | Testes executados e a passar (só então podem ser registados como realizados). |
| 9 | **Evolução** (opcional): filtros por estado/prioridade, categorias controladas, `CHECK`/índices na BD; migração do cliente para .NET moderno só por decisão explícita. | Ambos | A definir. |

## 9. Repositórios e commits analisados

| Repositório | URL | Ramo | Commit de referência | Data (UTC) | Mensagem | Nº de commits |
|---|---|---|---|---|---|---|
| ServiceTrack.API | https://github.com/alvaroteixeiramonteiro-dev/ServiceTrack.API | `main` (único) | **`1e88578`** | 01-11-2026 14:35 | Document API setup and finalize project cleanup | 7 |
| ServiceTrack.Client | https://github.com/alvaroteixeiramonteiro-dev/ServiceTrack.Client | `main` (único) | **`48b00c9`** | 01-11-2026 19:11 | Update ServiceTrack.Client.slnx | 10 |

Histórico resumido:
- **API:** modelo e esquema com GET → GET por id → POST → PUT → DELETE → documentação (30-10 a 01-11-2026).
- **Client:** setup → MDI e `FormPedidos` → ligação à API e grelha → seleção e campos → guardar/limpar → atualizar → eliminar → recarregar → layout e documentação (01-11-2026).

Ficheiros relevantes:
- **API:** `Controllers/PedidosController.cs`, `Models/Pedido.cs`, `Program.cs`, `database/servicetrack.sql`, `appsettings.json`, `Properties/launchSettings.json`, `ServiceTrack.API.http`, `README.md`.
- **Client:** `Services/ApiService.cs`, `FormPedidos.cs` e `FormPedidos.Designer.cs`, `principal.cs` e `principal.Designer.cs`, `Form1.*` (não usado), `Models/Pedido.cs`, `Program.cs`, `ServiceTrack.Client.csproj`, `packages.config`, `README.md`.

**Registo da sessão de análise:**
- O conetor GitHub não estava disponível nesta sessão.
- A análise foi feita por `git fetch`/clone dos repositórios públicos.
- Foi repetida uma segunda vez e os commits mantiveram-se `1e88578` e `48b00c9`.
- Não foi executada qualquer compilação ou teste.

## 10. Como retomar o desenvolvimento noutra conversa

1. **Confirmar o estado real antes de qualquer alteração.** Se o conetor GitHub estiver disponível, usá-lo. Caso contrário, pedir o clone/fetch dos dois repositórios públicos. Comparar o `HEAD` de `main` com `1e88578` (API) e `48b00c9` (Client).
2. **Se os commits forem diferentes**, listar os commits novos (`git log <ref>..HEAD`), reavaliar os ficheiros afetados e atualizar este documento antes de continuar. Não assumir que os problemas listados continuam por resolver sem verificar.
3. **Confirmar a estrutura, o ramo, as funcionalidades efetivamente implementadas e o objetivo do passo atual** antes de propor trabalho, em linha com as instruções globais.
4. **Escolher uma tarefa pequena** da secção 8 (por omissão, a prioridade 1) e decompô-la em passos, com teste rápido por passo e um commit por funcionalidade.
5. **Modo de trabalho:** responder em português europeu, atuar como mentor técnico e revisor, dar pistas, decomposição lógica e pequenos exemplos. **Não fornecer soluções completas ou código pronto sem pedido explícito.** Ao rever código, identificar o problema, explicar a causa e orientar uma correção incremental, respeitando o estilo existente.
6. **Regras técnicas a respeitar:**
   - Não inventar classes, tabelas, endpoints, esquemas ou ficheiros. Se não houver acesso ao código, assinalar a limitação.
   - O cliente é .NET Framework 4.7.2 (limite prático de C# 7.3); a API é .NET 10.
   - Preservar o design responsivo do WinForms e não impor maximização.
   - Nunca pedir nem reproduzir segredos em documentos partilháveis; usar variáveis de ambiente/User Secrets e exemplos fictícios.
   - Manter o README de cada repositório atualizado com cada funcionalidade.
7. **Para arrancar o ambiente local:** MySQL ativo → executar `database/servicetrack.sql` → correr a API com o perfil HTTPS (`https://localhost:7242`, Swagger em `/swagger`) → abrir `ServiceTrack.Client.slnx` no Visual Studio, restaurar pacotes, compilar e executar → menu **Pedidos → Gerir pedidos**.
8. **Registar apenas como realizado o que for efetivamente executado.** Testes e builds só passam a constar neste documento depois de serem feitos e confirmados pelo utilizador.
