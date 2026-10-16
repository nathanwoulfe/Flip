import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
  debug: true,
  input:
    "http://localhost:23901/umbraco/swagger/flip-management/swagger.json",
  output: {
    path: "generated",
  },
  plugins: [
    {
      name: "@hey-api/client-fetch",
      exportFromIndex: true,
      throwOnError: true,
    },
    {
      name: "@hey-api/typescript",
      enums: false,
    },
    {
      name: "@hey-api/sdk",
      operations: {
        containerName: (name) => `${name}Service`,
        strategy: "byTags",
      },
    }
  ],
});
