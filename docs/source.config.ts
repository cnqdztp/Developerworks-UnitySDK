import { defineDocs, defineConfig } from 'fumadocs-mdx/config';
import { i18n } from './lib/i18n';

export default defineConfig({
  mdxOptions: {
    rehypeCodeOptions: {
      themes: {
        light: 'github-light',
        dark: 'github-dark',
      },
    },
  },
  i18n,
  lastModifiedTime: 'git',
});

export const { docs, meta } = defineDocs({
  dir: 'content/docs',
});
