# UI tools

> [!WARNING]
> This tool is entirely AI-generated and has not been reviewed to the project's usual standards.
> It is a local development aid only — not production code — and is excluded from SonarQube analysis.

Headless Playwright helper to look at and interact with the locally running frontend
(start the Aspire AppHost first). Used for design work by humans and AI agents alike.

```sh
npm run ui -- login                                         # (re)login, session stored in .auth/
npm run ui -- shot /Lobbies --viewport all --full           # screenshots -> .screenshots/
npm run ui -- run steps.json --viewport mobile              # scripted interaction
```

Options: `--viewport desktop|tablet|mobile|all`, `--theme light|dark|all` (browser `prefers-color-scheme`),
`--full`, `--name <prefix>`. Console errors and page errors are printed.

Login uses the seeded dev user (`UI_USER`/`UI_PASS` env vars override; default `test` / `Pass123$`).
An expired session is renewed automatically.

## Steps file

A JSON array of single-key objects, executed in order:

```json
[
  { "goto": "/Lobbies" },
  { "fill": { "selector": "input[placeholder^='Search']", "value": "hjk" } },
  { "click": "text=hjk" },
  { "waitFor": "h1" },
  { "hover": "button" },
  { "press": "Escape" },
  { "wait": 500 },
  { "text": "main" },
  { "shot": "lobby-preview" }
]
```
