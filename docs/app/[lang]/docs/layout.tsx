import { DocsLayout } from 'fumadocs-ui/layout';
import type { ReactNode } from 'react';
import { source } from '@/lib/source';
import { i18n, languages } from '@/lib/i18n';

export default function Layout({
  params,
  children,
}: {
  params: { lang: string };
  children: ReactNode;
}) {
  const isZh = params.lang === 'zh';

  return (
    <DocsLayout
      tree={source.pageTree[params.lang]}
      nav={{
        title: isZh ? 'Developerworks SDK' : 'Developerworks SDK',
        url: `/${params.lang}`,
      }}
      i18n={{
        locale: params.lang,
        languages: languages.map(lang => ({
          name: lang.name,
          locale: lang.code,
        })),
      }}
      sidebar={{
        defaultOpenLevel: 0,
        banner: (
          <div className="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
            <p className="text-sm">
              {isZh
                ? '🚀 Unity SDK v0.1.7.2-beta'
                : '🚀 Unity SDK v0.1.7.2-beta'}
            </p>
          </div>
        ),
      }}
    >
      {children}
    </DocsLayout>
  );
}
