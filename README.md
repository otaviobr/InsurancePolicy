# InsurancePolicy

## Instuções para rodar a app
Para rodar, é necessário utilizar WSL. Rode o comando "docker compose up" que o banco e a api serão iniciados.

# Exemplo de requests

## Simulação
### Endpoint: /Simulation
## payload
{
  "insuredName": "Otavio Santos",
  "insuredDateOfBirth": "1957-01-31",
  "brokerName": "Otavio",
  "carModel": "500 Cult 1.4 Flex 8V EVO Dualogic",
  "carBrand": "Fiat",
  "carModelYear": 2014
}

### Response
{
  "policyId": "15be379258a2407183b2bb5f883348c8",
  "status": "Pending",
  "insuredName": "Otavio Santos",
  "insuredDateOfBirth": "1957-01-31",
  "premiumAmount": 691.98,
  "coverageAmount": 47723,
  "brokerName": "Otavio",
  "coverageStartDate": "2025-07-02",
  "coverageEndDate": "2026-07-02",
  "issueDate": "0001-01-01T00:00:00",
  "validUntil": "2025-07-09T00:00:00Z",
  "carPrice": 47723,
  "carModel": "500 Cult 1.4 Flex 8V EVO Dualogic",
  "carBrand": "Fiat",
  "carModelYear": 2014
}

## Ativação do seguro
### Endpoint: PolicyRegistration/UpdatePolicyStatus
### Parametros: 
- policyId=15be379258a2407183b2bb5f883348c8
- status=Active

### Response
{
  "policyId": "15be379258a2407183b2bb5f883348c8",
  "status": "Active", // status ativado
  "insuredName": "Otavio Santos",
  "insuredDateOfBirth": "1957-01-31",
  "premiumAmount": 691.98,
  "coverageAmount": 47723,
  "brokerName": "Otavio",
  "coverageStartDate": "2025-07-02",
  "coverageEndDate": "2026-07-02",
  "issueDate": "2025-07-02T15:22:29Z", // data de inicio da cobertura
  "validUntil": "0001-01-01T00:00:00Z",
  "carPrice": 47723,
  "carModel": "500 Cult 1.4 Flex 8V EVO Dualogic",
  "carBrand": "Fiat",
  "carModelYear": 2014
}

# Testes de performance
## Configuração debug
### 320 usuarios durante 1 minuto
![image](https://github.com/user-attachments/assets/b30f25a1-d5ea-4058-b996-d9d3f5d6cad9)

## Configuração Release
### 320 usuarios durante 1 minuto
![image](https://github.com/user-attachments/assets/930d2853-c9ec-452d-82b2-80bdb2eb7333)



