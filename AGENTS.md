# Orientações do projeto

Antes de criar ou modificar UI, leia `UI_GUIDELINES.md` e `DESIGN_SYSTEM.md` integralmente.
Consulte `README.md` para escopo, arquitetura e autoridade LAN.
Preserve transformações autorais, referências e alterações existentes do usuário.
Não regenere cenas para mudanças pontuais. Use componentes e layouts reutilizáveis.
Não faça builds sem autorização explícita. Prefira verificações focadas no Editor.
Escolha os controles conforme a tarefa e o GDD específico; não repita painéis de edição em galerias de observação.
Todo GDD novo ou revisado deve listar explicitamente interfaces/HUD, rótulos e controles 3D, sua finalidade, localização, ações, visibilidade inicial e simultânea, conforme `docs/gdd-template.md`. Justifique cada painel e priorize interação com os objetos; não introduza interfaces adicionais silenciosamente na implementação.
Antes de implementar UI nova ou reformulada, incluir no GDD imagens das interfaces propostas e de seus estados relevantes, com visão espacial, manipulação, consulta e erros/confirmações quando aplicáveis. HTML capturado ou imagem conceitual são válidos; só texto ou link para HTML não substitui as imagens. Distinguir proposta de cena implementada e manter as prévias coerentes com o inventário.
Salve capturas e mídias geradas nas pastas ignoradas pelo Git, conforme `docs/development-guidelines.md`. Preserve o versionamento dos assets usados pelo aplicativo e seus `.meta`.
Toda interação deve ter motion discreto conforme DESIGN_SYSTEM.md: feedback de foco/ação/estado, preferência reduzida e restauração ao ocultar. Preserve áreas clicáveis, colliders, transforms científicos e câmera; reutilize Shared/Interaction sem duplicar efeitos.
