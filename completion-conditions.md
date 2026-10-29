# Условия завершения для `/goal`

Три прогона, по одному на этап. Каждое условие состоит из трёх частей:
измеримое состояние, явная проверка и ограничения — плюс предохранитель.

## Почему условия выглядят именно так

Оценщик `/goal` видит **только текст разговора**. Он не запускает тесты и не читает файлы.
Поэтому условие не может опираться на слова вроде «игра работает»: агент напишет «работает»,
и цель закроется. Условие опирается на одну вещь — вывод `verify.ps1` и его код выхода.

Второе: самый короткий путь к зелёной проверке — ослабить саму проверку. Поэтому
`verify.ps1`, `Directory.Build.props` и `REQUIREMENTS.md` защищены git-тегом `0.0.0`,
и скрипт падает, если они изменились.

## Перед первым прогоном

Проверить, что тег на месте и указывает на последний коммит:

```bash
git -C F:/Projects/Snake rev-parse --short "0.0.0^{commit}" HEAD
```

Обе строки должны совпасть. Если требования меняются намеренно, тег переносит человек:
`git tag -f 0.0.0`.

Проверить, что старт красный:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ./verify.ps1 -Stage 1 -MinTests 25
```

Должно быть `VERIFY FAILED` и код выхода 1. Если на старте зелено — проверка ничего не проверяет.

---

## Этап 1 — `Snake.Core`: правила игры

```text
/goal Implement stage 1 of the Snake game in F:\Projects\Snake. Read REQUIREMENTS.md for the rules and tracker.md for the task list. Scope: tasks T01 through T10 only, inside src/Snake.Core and tests/Snake.Core.Tests. You design the public API of Snake.Core yourself. Done when all four hold: (1) the command `powershell -NoProfile -ExecutionPolicy Bypass -File ./verify.ps1 -Stage 1 -MinTests 25` was run in this session and its output ends with VERIFY PASSED and exit code 0; (2) tracker.md shows T01 through T10 with status done; (3) every check in section 7.2 of REQUIREMENTS.md for stage 1 has its own test; (4) `git status --porcelain` prints nothing because all work is committed. Constraints: do not modify verify.ps1, Directory.Build.props or REQUIREMENTS.md; do not lower the coverage threshold or MinTests; do not add code to src/Snake.Wpf; Snake.Core must not reference any UI namespace. If you cannot finish a task, write the reason into a file named BLOCKED in the repository root and stop. Or stop after 7 turns or a BLOCKED file exists.
```

| Фрагмент | Роль |
|---|---|
| `verify.ps1 -Stage 1 -MinTests 25` ends with VERIFY PASSED, exit 0 | Измеримое состояние + явная проверка: вывод ложится в транскрипт |
| tracker.md T01…T10 `done` | Измеримое состояние, видно в файле и в diff |
| каждая проверка 7.2 имеет свой тест | Ограничение против «одного теста ради покрытия» |
| `git status --porcelain` пустой | Работа закоммичена, есть что ревьюить как diff |
| не менять `verify.ps1` и порог | Ограничение против ослабления проверки |
| `or stop after 7 turns or a BLOCKED file exists` | Предохранитель |

---

## Этап 2 — хранилище рекордов и настроек

```text
/goal Implement stage 2 of the Snake game in F:\Projects\Snake. Read REQUIREMENTS.md and tracker.md. Scope: tasks T11 through T16 only, inside src/Snake.Core and tests/Snake.Core.Tests. Done when all four hold: (1) the command `powershell -NoProfile -ExecutionPolicy Bypass -File ./verify.ps1 -Stage 2 -MinTests 40` was run in this session and its output ends with VERIFY PASSED and exit code 0; (2) tracker.md shows T01 through T16 with status done; (3) every check in section 7.2 of REQUIREMENTS.md for stage 2 has its own test, including a corrupted JSON file that must not throw; (4) `git status --porcelain` prints nothing. Constraints: do not modify verify.ps1, Directory.Build.props or REQUIREMENTS.md; do not lower the coverage threshold or MinTests; do not change any test written for stage 1; do not add code to src/Snake.Wpf; tests must not touch the real file system or the system clock. If you cannot finish a task, write the reason into a file named BLOCKED in the repository root and stop. Or stop after 7 turns or a BLOCKED file exists.
```

Отличия от этапа 1:

- `-MinTests 40` — к тестам первого этапа добавляются новые;
- трекер проверяется с T01, чтобы прошлый этап нельзя было откатить;
- запрет менять тесты первого этапа;
- отдельно названа проверка повреждённого JSON (FR-41).

---

## Этап 3 — `Snake.Wpf`: интерфейс

⚠️ **Этот этап проверяется машиной слабо.** Юнит-тестами интерфейс не покрыть, и порог
покрытия к нему не относится. Оценщик может убедиться, что проект собрался и приложение
стартовало. Что змейка выглядит правильно и клавиши работают — нет. Основная проверка
здесь ваша, по разделу 7.3 требований.

```text
/goal Implement stage 3 of the Snake game in F:\Projects\Snake. Read REQUIREMENTS.md and tracker.md. Scope: tasks T17 through T25, inside src/Snake.Wpf. Done when all five hold: (1) the command `powershell -NoProfile -ExecutionPolicy Bypass -File ./verify.ps1 -Stage 0 -MinTests 40` was run in this session and its output ends with VERIFY PASSED and exit code 0; (2) `dotnet build src/Snake.Wpf/Snake.Wpf.csproj -c Release` exits with code 0 and prints no warning; (3) tracker.md shows T01 through T25 with status done; (4) README.md explains how to build and run the game; (5) `git status --porcelain` prints nothing. Constraints: do not modify verify.ps1, Directory.Build.props or REQUIREMENTS.md; do not change any existing test; all game logic stays in Snake.Core, the WPF layer only draws, reads input and owns the DispatcherTimer; no code-behind may contain game rules. If you cannot finish a task, write the reason into a file named BLOCKED in the repository root and stop. Or stop after 7 turns or a BLOCKED file exists.
```

---

## Длина условий

Лимит `/goal` — 4000 символов. Запас есть:

| Этап | Символов |
|---|---|
| 1 | ~1150 |
| 2 | ~1150 |
| 3 | ~1200 |

## Что делать после каждого прогона

1. Посмотреть панель: голый `/goal`, без текста после него. Текст заменит цель.
2. Если причина оценщика повторяется без прогресса — `/goal clear` и разобраться руками.
3. Проверить результат как `git diff 0.0.0`, а не по итоговому тексту агента.
4. Прочитать код тестов. `verify.ps1` считает строки покрытия, а не смысл проверок.
5. Если появился файл `BLOCKED` — прочитать причину, решить вопрос, удалить файл.

## Чего эти условия не закрывают

- **Качество тестов.** Покрытие 80 % набирается и слабыми тестами. Нужно ревью.
- **Трекер самооценочный.** Галочки `done` ставит тот же агент. Связь с кодом даёт
  только `verify.ps1` и ваш diff.
- **Мягкий предохранитель.** `or stop after 7 turns` оценивает модель по транскрипту,
  и он обнуляется при возобновлении сессии. Твёрдую границу расходов нужно держать
  отдельно, в настройках аккаунта.
