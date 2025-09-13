# KernelBuilder
KernelBuilder is an automatic generator for creating simple Hashcat-compatible plugins. They are [famously](https://github.com/hashcat/hashcat/blob/master/docs/hashcat-plugin-development-guide.md) difficult to get started with, so this project can provide everyone with an equal opportunity to create any primitive plugin they like, without needing to know how to write CUDA/OpenCL and C or any cryptography.

### Usage ###
```
KernelBuilder <algorithm> <ID>
```

Supported Algorithms:
```
MD5
SHA1
SHA224
SHA256
CUT
```

Example algorithms:
```
md5($plain)
md5($plain.$salt)
sha256(md5(sha1($plain)).sha1($plain.$salt).sha256(md5($plain)).md5($plain).sha1(sha1($plain)).sha256(sha1($plain)))
md5(CUT_16sha256($plain))
sha224(sha1($plain))
```

### Generated code quality ###
As mentioned at the top of each file, these files should NOT be submitted to Hashcat master. These plugins currently break *many* of Hashcat's [contribution rules](https://github.com/hashcat/hashcat/tree/master?tab=readme-ov-file#contributing) and will be rejected by the reviewers.
KernelBuilder does not currently support self-test and is auto-disabled in each plugin. This may cause silent accuracy problems if KernelBuilder acts unexpectedly and encounters a bug. Hashcat plugins are **very** complex, so while this tool is heavily tested, please do be careful of any [issues](https://github.com/PenguinKeeper7/KernelBuilder/issues).

### Module context size error / Kernel errors ###
These fatal errors in Hashcat are likely caused by Hashcat's frequent backwards compatibility-breaking changes in both modules and kernels. When you see these, please make sure you are using the [latest Hashcat master code](https://github.com/hashcat/hashcat/). If it persists, please submit a [KernelBuilder GitHub issue](https://github.com/PenguinKeeper7/KernelBuilder/issues).

### Contribution ###
- Use tabs, not spaces
- Spaces after comments, ie "// Comment"
- PascalCase for functions, camelCase for variables
- Vibe-coded PRs are fine, just make sure they're functional and readable
- Comment-only PRs are fine
- Single-line if's are fine, just don't make them too messy

### Enjoy!! ###