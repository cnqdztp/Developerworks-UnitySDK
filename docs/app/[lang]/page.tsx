import Link from 'next/link';

export default function HomePage({ params }: { params: { lang: string } }) {
  const isZh = params.lang === 'zh';

  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-8 md:p-24 bg-gradient-to-b from-gray-50 to-white dark:from-gray-900 dark:to-gray-800">
      <div className="max-w-6xl w-full">
        <div className="text-center mb-16">
          <h1 className="text-5xl md:text-6xl font-bold mb-6 bg-clip-text text-transparent bg-gradient-to-r from-blue-600 to-purple-600">
            {isZh ? 'Developerworks SDK' : 'Developerworks SDK'}
          </h1>
          <p className="text-xl md:text-2xl mb-4 text-gray-700 dark:text-gray-300">
            {isZh
              ? '官方 SDK 文档'
              : 'Official SDK Documentation'}
          </p>
          <p className="text-base md:text-lg text-gray-600 dark:text-gray-400">
            {isZh
              ? '为您的应用集成强大的 AI 能力 - 支持多个平台和游戏引擎'
              : 'Integrate powerful AI capabilities into your apps - Support for multiple platforms'}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <Link
            href={`/${params.lang}/docs/unity`}
            className="group rounded-xl border-2 border-blue-500 bg-white dark:bg-gray-800 p-6 shadow-lg transition-all hover:shadow-2xl hover:scale-105"
          >
            <div className="flex items-start justify-between mb-4">
              <h2 className="text-3xl font-bold text-blue-600 dark:text-blue-400">Unity SDK</h2>
              <span className="inline-block px-3 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200">
                {isZh ? '可用' : 'Available'}
              </span>
            </div>
            <p className="text-gray-600 dark:text-gray-300 mb-4">
              {isZh
                ? 'Unity 游戏引擎的完整 AI SDK - 支持聊天、图像生成、语音转录等功能'
                : 'Complete AI SDK for Unity game engine - Chat, image generation, audio transcription & more'}
            </p>
            <div className="flex items-center text-blue-600 dark:text-blue-400 font-medium">
              {isZh ? '查看文档' : 'View Documentation'}
              <span className="ml-2 transition-transform group-hover:translate-x-2">→</span>
            </div>
            <div className="mt-4 pt-4 border-t border-gray-200 dark:border-gray-700 text-sm text-gray-500">
              v0.1.7.2-beta
            </div>
          </Link>

          <div className="rounded-xl border-2 border-gray-300 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50 p-6 shadow-lg opacity-70">
            <div className="flex items-start justify-between mb-4">
              <h2 className="text-3xl font-bold text-gray-600 dark:text-gray-400">Unreal SDK</h2>
              <span className="inline-block px-3 py-1 text-xs font-semibold rounded-full bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200">
                {isZh ? '即将推出' : 'Coming Soon'}
              </span>
            </div>
            <p className="text-gray-500 dark:text-gray-400 mb-4">
              {isZh
                ? 'Unreal Engine 的 AI SDK 文档即将推出'
                : 'AI SDK for Unreal Engine - Documentation coming soon'}
            </p>
            <div className="text-gray-400 font-medium">
              {isZh ? '开发中...' : 'In Development...'}
            </div>
          </div>
        </div>

        <div className="mt-16 text-center space-y-6">
          <div className="flex justify-center gap-8">
            <Link
              href={params.lang === 'en' ? '/zh' : '/en'}
              className="text-lg font-medium text-blue-600 dark:text-blue-400 hover:underline"
            >
              {params.lang === 'en' ? '🇨🇳 中文' : '🇺🇸 English'}
            </Link>
            <a
              href="https://github.com/cnqdztp/Developerworks-UnitySDK"
              target="_blank"
              rel="noopener noreferrer"
              className="text-lg font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400"
            >
              GitHub
            </a>
            <a
              href="https://developerworks.agentlandlab.com"
              target="_blank"
              rel="noopener noreferrer"
              className="text-lg font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400"
            >
              {isZh ? '开发者平台' : 'Developer Platform'}
            </a>
          </div>
        </div>
      </div>
    </main>
  );
}
