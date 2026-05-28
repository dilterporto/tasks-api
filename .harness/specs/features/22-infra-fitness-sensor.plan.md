# Plan: #22 — Create infra-fitness sensor and update spec #10 sensors section

**Spec:** `.harness/specs/features/22-infra-fitness-sensor.md`  
**Status:** Draft  
**Created:** 2026-05-28

## Tasks

- [ ] **task-1** · agent: `architect`
  Criar `.harness/sensors/infra-fitness.md` com quatro grupos de checks (Structural Validation, Security & Credentials, Module Parameterization, Pipeline & Documentation), cada um com ao menos um comando executável mapeado ao Must/Must not correspondente da spec #10.
  _depends on: —_
  _covers: AC-1, AC-2, AC-3_

- [ ] **task-2** · agent: `architect`
  Substituir a seção `## Sensors` da spec #10 pelos checks concretos do `infra-fitness.md` e atualizar o campo `**Status:**` da spec #10 para `Ready`.
  _depends on: task-1_
  _covers: AC-4, AC-5_
