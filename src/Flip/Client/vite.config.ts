import { defineConfig } from "vite";
import { warn } from "console";

export default defineConfig({
  build: {
    lib: {
      entry: "src/index.ts", // your web component source file
      formats: ["es"],
    },
    outDir: "../wwwroot", // all compiled files will be placed here
    emptyOutDir: false,
    sourcemap: true,
    rolldownOptions: {
      external: [/^@umbraco/], // ignore the Umbraco Backoffice package in the build
      onwarn: (err) => warn(err),
      output: {
        chunkFileNames: "[name].js",
      },
    },
  },
});
