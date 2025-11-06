import { defineI18n } from 'fumadocs-core/i18n';

export const i18n = defineI18n({
  defaultLanguage: 'en',
  languages: ['en', 'zh'],
  fallbackLanguage: 'en',
});

export const languages = [
  { code: 'en', name: 'English' },
  { code: 'zh', name: '中文' },
];
