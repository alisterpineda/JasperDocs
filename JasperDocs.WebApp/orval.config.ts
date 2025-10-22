import { defineConfig } from 'orval';

export default defineConfig({
  jasperdocs: {
    input: '../JasperDocs.WebApi/JasperDocs.WebApi.json',
    output: {
      target: './src/api/generated/api.ts',
      client: 'react-query',
      mode: 'tags-split',
      override: {
        mutator: {
          path: './src/api/axios-instance.ts',
          name: 'customAxiosInstance',
        },
      },
    },
  },
});
