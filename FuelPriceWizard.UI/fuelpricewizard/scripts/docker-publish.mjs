// Builds and pushes the UI image to the registry under a chosen tag.
//
//   npm run docker:publish                 -> tag "latest" (default)
//   npm run docker:publish -- <tag>        -> any tag, e.g. v1.2.3
//   npm run docker:publish:experimental    -> tag "experimental" (alias)
//
// A Node helper (rather than an inline npm script) keeps the tag a single
// variable used in both the build and push, and works the same on Windows
// and on the Linux CI runners.
import { execSync } from 'node:child_process';

const IMAGE = 'registry.mrausch-schott.com/fuelpricewizard-ui';

// Tag precedence: CLI arg (after `--`) > DOCKER_TAG env var > "latest".
const tag = (process.argv[2] ?? process.env.DOCKER_TAG ?? 'latest').trim();

// Docker tag grammar: word char first, then [A-Za-z0-9._-], max 128 chars.
if (!/^[A-Za-z0-9_][A-Za-z0-9._-]{0,127}$/.test(tag)) {
  console.error(`Invalid image tag: "${tag}"`);
  process.exit(1);
}

const ref = `${IMAGE}:${tag}`;
console.log(`Publishing ${ref}\n`);
execSync(`docker build -t ${ref} .`, { stdio: 'inherit' });
execSync(`docker push ${ref}`, { stdio: 'inherit' });
