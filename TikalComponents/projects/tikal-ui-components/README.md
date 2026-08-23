# TikalUiComponents

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 22.1.0.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the library, run:

```bash
ng build tikal-ui-components
```

This command will compile your project, and the build artifacts will be placed in the `dist/` directory.

### Publishing the Library

Once the project is built, you can publish your library by following these steps:

1. Navigate to the `dist` directory:

   ```bash
   cd dist/tikal-ui-components
   ```

2. Run the `npm publish` command to publish your library to the npm registry:
   ```bash
   npm publish
   ```

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

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
