# Tikal-ui-components

Reusable Angular UI components for the Tikal board game, built on PrimeNG with the Tikal theme. Everything consumers can use is exported from `src/public-api.ts`; translations live in `public/i18n`.

### PrimeNG license

This package does not contain a PrimeNG license key. Configure PrimeNG in the
Angular application that consumes the package, using a key supplied through
that application's deployment configuration:

```ts
// app.config.ts
import { ApplicationConfig } from '@angular/core';
import { providePrimeNG } from 'primeng/config';
import { environment } from './environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    providePrimeNG({
      license: environment.primengLicense,
    }),
  ],
};
```

Keep the key out of this package's source, `package.json`, and published build
artifacts. Browser applications necessarily expose the key at runtime, so
environment configuration prevents accidental source-control and npm leaks;
it is not a mechanism for hiding the key from end users.

For local Storybook development, copy `.env.example` to `.env.local` and set
`STORYBOOK_PRIMENG_LICENSE`. CI can provide the same variable without storing
the key in the repository.
