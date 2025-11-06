import type { ReactNode } from 'react';
import { i18n } from '@/lib/i18n';

export async function generateStaticParams() {
  return i18n.languages.map((lang) => ({ lang }));
}

export default function LangLayout({
  params,
  children,
}: {
  params: { lang: string };
  children: ReactNode;
}) {
  return children;
}
