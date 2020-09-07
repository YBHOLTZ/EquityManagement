# EquityManagement

- Qualquer request da API deverá conter um token no header, que será validado no banco. O script de criação das tabelas contém o primeiro token
- Sempre será retornado um json para qualquer request.
- api/brand/6/equities vai retornar todos os patrimônios com a marca de ID 6.
- Não foi usado o EF para manipulação do banco. Foi feito o "YboEntityFramework" que faz o básico que o EF faria.
- Estrutura de retorno quando houver erro ou por algum motivo o resultado retornado não for o esperado:
{
    "message": "",
    "status": 
}
status: 200 não houve erros, a mensagem vai dizer o que aconteceu.
status: 400 houve erro.

Ex:
{
    "message": "Ok. Removed.",
    "status": 200
}

ou 

{
    "message": "ERROR: The Brand name already exists.",
    "status": 400
}



- Exemplo de retorno api/equity:

[
    {
        "id": 1,
        "name": "Equity-001",
        "brandID": 4,
        "description": "description - sd",
        "register": 2327
    },
    {
        "id": 2,
        "name": "Equity-001",
        "brandID": 4,
        "description": "description - sd",
        "register": 2318
    },
]

- Exemplos de body para requests;

Body para:
POST api/equity
PUT api/equity/5
{
  "name": "teste",
  "brandID": 5,
  "description": "sobre..."
}

Body para:
POST api/brand
PUT api/brand/5
{
 "name" : "brand-01'
}
