
using Microsoft.Extensions.FileSystemGlobbing;
using Shiron.Docs.Engine;
using Shiron.Docs.Engine.Builder;
using Shiron.Docs.Engine.Extractor;
using Shiron.Docs.Engine.Model;
using Shiron.Docs.Engine.PM;
using Shiron.Docs.Engine.Vite;
using Spectre.Console.Cli;

namespace Shiron.Docs.Cli.Services;

public static class DefaultFileContents {
    public static readonly string AppTsx = """
    import { useState } from "preact/hooks";
    import { Router, type RouterOnChangeArgs } from "preact-router";

    import { ThemeProvider } from "./contexts/ThemeContext";
    import { Layout } from "./layouts/Layout";
    import { PAGES } from "./utils/pages";

    export function App() {
    	const [url, setUrl] = useState(
    		typeof window !== "undefined" ? window.location.pathname : "/",
    	);

    	const handleRoute = (e: RouterOnChangeArgs) => {
    		setUrl(e.url);
    		if (typeof window !== "undefined") {
    			if (!window.location.hash) {
    				window.scrollTo(0, 0);
    			}
    		}
    	};

    	return (
    		<ThemeProvider>
    			<Layout currentPath={url}>
    				<Router onChange={handleRoute}>
    					{PAGES.map((page) => (
    						<page.Content key={page.id} path={page.path} />
    					))}
    				</Router>
    			</Layout>
    		</ThemeProvider>
    	);
    }
    """;

    public static readonly string ConfigTsx = """
    import type { IconMap } from "./utils/icons";

    export const CONFIG = {
    	branding: {
    		appTitle: "Shiron.Docs",
    		appTitleShort: "Shiron",
    		libraryName: "Shiron.Logging",
    		tagline:
    			"High-performance interception logging library for .NET applications.",
    		logoIcon: "Package" as keyof typeof IconMap,
    		copyright: `© ${new Date().getFullYear()} Shiron. Licensed under MIT.`,
    	},
    	navigation: {
    		externalLinks: {
    			github: "https://github.com/iamshiron",
    			repository: "https://github.com/iamshiron/Shiron.Logging",
    			profile: "https://github.com/iamshiron",
    		},
    		footerLinks: [
    			{ label: "Profile", href: "https://github.com/iamshiron" },
    			{ label: "Repository", href: "#" },
    			{ label: "License", href: "#" },
    			{ label: "Contact", href: "#" },
    		],
    	},
    };
    """;

    public static readonly string MainTsx = """
    import { hydrate, prerender as ssr } from "preact-iso";
    import "./index.css";
    import { App } from "./app";

    if (typeof window !== "undefined") {
    	hydrate(<App />, document.getElementById("app")!);
    }

    export async function prerender(data: any) {
    	const { html, links } = await ssr(<App {...data} />);
    	return {
    		html,
    		links,
    	};
    }
    """;

    public static readonly string IndexCss = """
    @import "tailwindcss";

    @theme {
    	/* COLORS */
    	--color-accent-primary: #a855f7;
    	--color-accent-dark: #7e22ce;
    	--color-accent-glow: rgba(168, 85, 247, 0.4);

    	--color-bg-main: var(--bg-color);
    	--color-glass-surface: var(--glass-surface);
    	--color-glass-border: var(--glass-border);
    	--color-glass-highlight: var(--glass-highlight);

    	--color-text-bright: var(--text-bright);
    	--color-text-main: var(--text-main);
    	--color-text-muted: var(--text-muted);
    	--color-text-code: var(--text-code);

    	--color-sidebar-bg: var(--sidebar-bg);
    	--color-header-bg: var(--header-bg);
    	--color-code-bg: var(--code-bg);

    	--color-semantic-warning: var(--semantic-warning);
    	--color-semantic-warning-bg: var(--semantic-warning-bg);

    	/* Syntax Highlighting Tokens */
    	--color-sh-keyword: var(--sh-keyword);
    	--color-sh-type: var(--sh-type);
    	--color-sh-string: var(--sh-string);
    	--color-sh-comment: var(--sh-comment);
    	--color-sh-func: var(--sh-func);
    	--color-sh-num: var(--sh-num);

    	/* SPACING & SIZES */
    	--spacing-header-height: 64px;

    	/* RADIUS */
    	--radius-lg: 16px;
    	--radius-md: 8px;
    	--radius-sm: 4px;

    	/* ANIMATIONS */
    	--animate-fade-in: fadeIn 0.3s ease-out;

    	@keyframes fadeIn {
    		from {
    			opacity: 0;
    			transform: translateY(10px);
    		}
    		to {
    			opacity: 1;
    			transform: translateY(0);
    		}
    	}

    	@keyframes slideDown {
    		from {
    			opacity: 0;
    			transform: translateY(-10px);
    		}
    		to {
    			opacity: 1;
    			transform: translateY(0);
    		}
    	}
    }

    :root {
    	/* DARK THEME (Default) */
    	--bg-color: #0f0c19;
    	--bg-gradient: radial-gradient(circle at 10% 20%, #2e1065 0%, #0f0c19 40%);

    	--glass-surface: rgba(255, 255, 255, 0.03);
    	--glass-border: rgba(255, 255, 255, 0.08);
    	--glass-highlight: rgba(255, 255, 255, 0.15);

    	--sidebar-bg: rgba(15, 12, 25, 0.2);
    	--header-bg: rgba(15, 12, 25, 0.8);
    	--code-bg: rgba(0, 0, 0, 0.3);

    	--accent-primary: #a855f7;
    	--accent-glow: rgba(168, 85, 247, 0.4);
    	--accent-dark: #7e22ce;

    	--text-bright: #f8fafc;
    	--text-main: #94a3b8;
    	--text-muted: #64748b;
    	--text-code: #cbd5e1;

    	--semantic-warning: #f59e0b;
    	--semantic-warning-bg: rgba(245, 158, 11, 0.1);

    	/* Dark Theme Syntax */
    	--sh-keyword: #c084fc;
    	--sh-type: #60a5fa;
    	--sh-string: #4ade80;
    	--sh-comment: #64748b;
    	--sh-func: #f0abfc;
    	--sh-num: #fca5a5;
    }

    [data-theme="light"] {
    	--bg-color: #f8fafc;
    	--bg-gradient: radial-gradient(circle at 10% 20%, #f3e8ff 0%, #f8fafc 40%);

    	--glass-surface: rgba(255, 255, 255, 0.7);
    	--glass-border: rgba(0, 0, 0, 0.06);
    	--glass-highlight: rgba(255, 255, 255, 0.9);

    	--sidebar-bg: rgba(255, 255, 255, 0.5);
    	--header-bg: rgba(255, 255, 255, 0.85);
    	--code-bg: rgba(0, 0, 0, 0.04);

    	--text-bright: #0f172a;
    	--text-main: #475569;
    	--text-muted: #64748b;
    	--text-code: #334155;

    	--accent-glow: rgba(168, 85, 247, 0.15);

    	/* Light Theme Syntax */
    	--sh-keyword: #9333ea;
    	--sh-type: #2563eb;
    	--sh-string: #16a34a;
    	--sh-comment: #94a3b8;
    	--sh-func: #c026d3;
    	--sh-num: #d946ef;
    }

    @layer base {
    	body {
    		@apply font-sans bg-bg-main text-text-main leading-relaxed overflow-x-hidden min-h-screen flex flex-col transition-colors duration-300;
    		background-image: var(--bg-gradient);
    		background-attachment: fixed;
    	}

    	#app {
    		@apply flex flex-col min-h-screen;
    	}

    	a {
    		@apply no-underline text-inherit transition-colors duration-200;
    	}

    	ul {
    		@apply list-none;
    	}

    	/* Typography for content */
    	h1 { @apply text-4xl font-bold mb-3 text-text-bright; }
    	h2 { @apply text-3xl font-semibold mt-5 mb-2.5 text-text-bright; }
    	h3 { @apply text-xl font-semibold mt-4 mb-2 text-text-bright; }
    	p { @apply mb-2.5 text-text-main; }
    	strong { @apply text-text-bright font-semibold; }

    	code {
    		@apply font-mono bg-code-bg px-1.5 py-0.5 rounded-sm text-[0.85em] text-accent-primary;
    	}

    	pre {
    		@apply bg-code-bg p-4 rounded-md overflow-x-auto border border-glass-border my-4;
    	}

    	pre code {
    		@apply bg-transparent p-0 text-text-code block;
    	}
    }

    @layer utilities {
    	/* Scrollbar */
    	::-webkit-scrollbar { @apply w-2 h-2; }
    	::-webkit-scrollbar-track { @apply bg-transparent; }
    	::-webkit-scrollbar-thumb { @apply bg-glass-border rounded-sm hover:bg-glass-highlight; }

    	/* Global semantic classes */
    	.alert {
    		@apply p-3 rounded-md border-l-4 border-semantic-warning bg-semantic-warning-bg my-4 text-[#ffe4b5] data-[theme=light]:text-[#854d0e];
    	}
    	.alert strong { @apply text-semantic-warning; }

    	.breadcrumbs {
    		@apply flex items-center gap-2 text-sm text-text-muted mb-3 font-mono whitespace-nowrap overflow-x-auto;
    	}
    	.breadcrumbs .active { @apply text-accent-primary; }

    	.api-table {
    		@apply w-full border-collapse my-2 text-[0.95rem];
    	}
    	.api-table th {
    		@apply text-left p-3 text-text-muted border-b border-glass-border font-medium;
    	}
    	.api-table td {
    		@apply p-3 border-b border-glass-border align-top;
    	}
    	.api-table tr:last-child td { @apply border-b-0; }

    	/* UNIVERSAL SYNTAX HIGHLIGHTING CLASSES */
    	/* Both tok-* and raw classes used in signatures */
    	.tok-kw, .keyword { @apply text-sh-keyword; }
    	.tok-type, .type { @apply text-sh-type; }
    	.tok-str, .string { @apply text-sh-string; }
    	.tok-com, .comment { @apply text-sh-comment italic; }
    	.tok-func, .identifier, .function { @apply text-sh-func; }
    	.tok-num, .number { @apply text-sh-num; }
    }
    """;

    public static readonly string ViteConfigJs = """
    import { defineConfig } from "vite";
    import preact from "@preact/preset-vite";

    export default defineConfig({
    	plugins: [
    		preact({
    			prerender: {
    				enabled: true,
    				renderTarget: "#app",
    				additionalPrerenderRoutes: [
    					"/markdown-demo",
    					"/manual-installation",
    					"/config-strategies",
    					"/sec-encryption",
    					"/sec-roles",
    					"/api/class-loginjector",
    					"/mcp/intro",
    					"/mcp/setup",
    					"/mcp/resources",
    					"/mcp/tools",
    				],
    			},
    		}),
    	],
    	resolve: {
    		alias: {
    			react: "preact/compat",
    			"react-dom": "preact/compat",
    		},
    	},
    	css: {
    		postcss: "./postcss.config.js",
    	},
    });
    """;

    public static readonly string IndexHtml = """
    <!DOCTYPE html>
    <html lang="en">
      <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Shiron.Docs</title>
        <style>
          html, body { background: #0f0c19; margin: 0; }
        </style>
      </head>
      <body>
        <div id="app"><!--ssr-outlet--></div>
        <script type="module" src="/src/main.tsx" prerender></script>
      </body>
    </html>
    """;
}

public static class CLIServices {
    public static async Task BootstrapViteServerAsync(Config config) {
        var packageManager = new PNPMPackageManager();
        var viteBootstrap = new ViteBootstrap(new ViteBootstrapOptions() {
            OutputDir = config.OutputDirectory
        });

        await packageManager.InstallPackagesAsync(config.OutputDirectory);
    }
    public static async Task InitEmptyViteServer(Config config) {
        await File.WriteAllTextAsync(Path.Combine(config.OutputDirectory, "src", "app.tsx"), DefaultFileContents.AppTsx);
        await File.WriteAllTextAsync(Path.Combine(config.OutputDirectory, "src", "config.tsx"), DefaultFileContents.ConfigTsx);
        await File.WriteAllTextAsync(Path.Combine(config.OutputDirectory, "src", "main.tsx"), DefaultFileContents.MainTsx);
        await File.WriteAllTextAsync(Path.Combine(config.OutputDirectory, "src", "index.css"), DefaultFileContents.IndexCss);
        await File.WriteAllTextAsync(Path.Combine(config.OutputDirectory, "vite.config.ts"), DefaultFileContents.ViteConfigJs);
        await File.WriteAllTextAsync(Path.Combine(config.OutputDirectory, "index.html"), DefaultFileContents.IndexHtml);
    }

    public static async Task<string[]> GetProjectFilesAsync(Config config) {
        var fileMatcher = new Matcher();
        fileMatcher.AddIncludePatterns(config.Includes);
        fileMatcher.AddExcludePatterns(config.Excludes);

        var projectFiles = fileMatcher.GetResultsInFullPath(Directory.GetCurrentDirectory());
        return [.. projectFiles];
    }
    public static async Task<List<AssemblyData>> GenerateAssemblyInfo(string[] projectFiles) {
        CSharpExtractor.Init();
        var extractor = new CSharpExtractor();

        foreach (var projectFile in projectFiles) {
            extractor.AddProject(projectFile);
        }

        return await extractor.ExtractAsync();
    }

    public static async Task GenerateDocumenationSitesAsync(Config config, AssemblyData[] assemblies) {
        var builder = new TSXBuilder(Path.Combine(config.OutputDirectory, "src/pages/api"));
        foreach (var assembly in assemblies) {
            await builder.BuildAssemblyAsync(assembly);
        }
    }

    public static async Task BuildSiteAsync() { }
}
