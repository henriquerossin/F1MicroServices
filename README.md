# 🏎️ F1 Championship Simulator

Este projeto é uma **simulação completa de um campeonato de Fórmula 1**, desenvolvida utilizando **arquitetura de microserviços**, com foco em regras de negócio, evolução de equipes ao longo da temporada e separação clara de responsabilidades entre APIs.

O sistema simula desde a criação das equipes até a execução de todos os eventos de uma temporada, incluindo treinos, classificação, corrida e melhorias técnicas.

---

## 🧩 Arquitetura do Sistema

O projeto é composto por **4 APIs independentes**, cada uma com uma responsabilidade bem definida


---

## 🧪 Tecnologias Utilizadas

- **ASP.NET Core Web API**
- **Dapper**
- **SQL Server Management Studio**
- **MongoDB**
- **Arquitetura de Microserviços**
- **RESTful APIs**

---

## 🔌 APIs do Sistema

### 🟦 TeamAPI

Responsável por **gerenciar todos os dados estruturais das equipes**.

#### 📌 Entidades gerenciadas:
- Team
- Pilot
- Car
- Engineer
- Boss

#### 🏁 Estrutura de uma equipe:
- 2 carros
- 2 pilotos (1 piloto por carro)
- 4 engenheiros:
  - 2 por carro
  - 1 focado em coeficiente aerodinâmico
  - 1 focado em coeficiente de potência
- 2 chefes de equipe

#### ⚙️ Responsabilidades:
- Criar equipes completas (manual ou automática)
- Atualizar dados conforme o andamento do campeonato
- Gerenciar pontuação e colocação das equipes
- Garantir a integridade estrutural da equipe

📦 **Banco de dados:** SQL Server  
📦 **ORM:** Dapper

---

### 🟩 CompetitionAPI

Responsável pelo **controle da temporada**.

#### ⚙️ Responsabilidades:
- Definir os circuitos da temporada
- Iniciar a temporada
- Finalizar a temporada
- Controlar o estado geral do campeonato

📦 **Banco de dados:** SQL Server  
📦 **ORM:** Dapper

---

### 🟥 RaceAPI

Responsável por **executar os eventos de corrida** e armazenar todo o histórico da temporada.

#### 🏎️ Eventos simulados:
- Treino Livre 1
- Treino Livre 2
- Treino Livre 3
- Classificação
- Corrida

#### 📊 Funcionalidades:
- Execução completa dos eventos
- Cálculo de resultados
- Armazenamento do histórico de todos os eventos
- Registro do status das equipes ao longo da temporada

📦 **Banco de dados:** MongoDB

---

### 🟨 EngineerAPI

Responsável pela **evolução técnica dos carros e pilotos**.

#### ⚙️ Funcionalidades:
- Melhoria dos coeficientes dos carros:
  - Coeficiente Aerodinâmico
  - Coeficiente de Potência
- Evolução gradual a cada evento
- Redução do handicap dos pilotos ao longo da temporada

---
