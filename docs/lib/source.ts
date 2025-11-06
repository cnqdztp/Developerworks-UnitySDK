import { loader } from 'fumadocs-core/source';
import { createMDXSource } from 'fumadocs-mdx';
import { i18n } from '@/lib/i18n';
import { docs, meta } from '@/source.config';

export const source = loader({
  baseUrl: '/docs',
  i18n,
  source: createMDXSource(docs, meta),
});
