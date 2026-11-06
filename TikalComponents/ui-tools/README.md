# UI tools

> [!WARNING]
> This tool is entirely AI-generated and has not been reviewed to the project's usual standards.
> It is a local development aid only — not production code — and is excluded from SonarQube analysis.

Headless Playwright helper to look at the locally running Storybook (start the Aspire AppHost first).
Used for design work by humans and AI agents alike.

```sh
npm run ui -- list                          # all story ids
npm run ui -- list player                   # story ids containing "player"
npm run ui -- shot                          # every story, light + dark -> .screenshots/
npm run ui -- shot player-slot --theme dark # only matching stories, dark mode only
```

Options: `--theme light|dark|all` (default `all`), `--width <px>` (default `900`).
Set `UI_STORYBOOK_URL` to point at a different Storybook (default `https://localhost:6006`).

Each run writes one PNG per story and theme plus `.screenshots/sheet.png`, an overview of all
captured stories side by side (light | dark). Browser console errors are printed per story.
