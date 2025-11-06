# Developerworks Unity SDK Documentation

This is the documentation website for the Developerworks Unity SDK, built with [Fumadocs](https://fumadocs.vercel.app/).

## Features

- 🌐 **Multi-language Support**: English and Chinese (中文)
- 📚 **Comprehensive Documentation**: Complete API reference and guides
- 🎨 **Modern UI**: Built with Fumadocs UI and Tailwind CSS
- 🚀 **Fast**: Built on Next.js with static site generation
- 🔍 **Search**: Built-in document search functionality

## Getting Started

### Prerequisites

- Node.js 20 or higher
- npm, pnpm, yarn, or bun

### Installation

Install dependencies:

```bash
npm install
# or
pnpm install
# or
yarn install
# or
bun install
```

### Development

Run the development server:

```bash
npm run dev
# or
pnpm dev
# or
yarn dev
# or
bun dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

### Build

Build the documentation site:

```bash
npm run build
# or
pnpm build
# or
yarn build
# or
bun build
```

### Production

Start the production server:

```bash
npm start
# or
pnpm start
# or
yarn start
# or
bun start
```

## Project Structure

```
docs/
├── app/                    # Next.js app directory
│   ├── [lang]/            # Multi-language routes
│   │   ├── docs/          # Documentation pages
│   │   └── page.tsx       # Home page
│   ├── globals.css        # Global styles
│   └── layout.tsx         # Root layout
├── content/               # Documentation content
│   └── docs/
│       ├── en/           # English documentation
│       │   └── unity/    # Unity SDK docs
│       └── zh/           # Chinese documentation
│           └── unity/    # Unity SDK docs
├── lib/                   # Utility functions
│   ├── i18n.ts           # i18n configuration
│   └── source.ts         # Content source configuration
├── middleware.ts          # Next.js middleware for i18n
├── source.config.ts       # Fumadocs configuration
└── package.json
```

## Adding New Documentation

### For Unity SDK

1. Create a new `.mdx` file in `content/docs/en/unity/` for English
2. Create the corresponding Chinese version in `content/docs/zh/unity/`
3. Update `meta.json` in both language folders to include the new page
4. The page will automatically be included in the navigation

### For Other SDK Versions (Future)

1. Create new directories: `content/docs/en/[sdk-name]/` and `content/docs/zh/[sdk-name]/`
2. Add the documentation files
3. Update the home page to include the new SDK version

## Internationalization (i18n)

The site supports English (`en`) and Chinese (`zh`) by default. To add more languages:

1. Update `lib/i18n.ts` to include the new language
2. Create content directories for the new language in `content/docs/[lang]/`
3. Translate all documentation files

## License

This documentation is part of the Developerworks Unity SDK project.
