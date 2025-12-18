# Lang Editing Guide
[API/Lang.resx](API/Lang.resx) and [Celosia/Lang.resx](Celosia/Lang.resx) are the default (English) language files. To add a new language, copy- these into new files named `Lang.language_COUNTRY.resx` (eg `Lang.ja_JP.resx`) and change the content in the `<value>` tags (and nothing outside of them)

You can edit the `.resx` file in any editor you want, but it's recommended to use one that has syntax highlighting to make it easier to read, such as [VSCode](https://code.visualstudio.com/)

`<!--Text-->` is a comment that does nothing, but may contain helpful information

## Formatting guide

- `/c[color]` changes the color of the text (eg `/c[white]` or `/c[#ffffff]`)
  - Any color can be used. Some colors have special names, (TODO clarify)
- `/i[image]` embeds an image from [icons.json](Game/Content/img/icons.json) (eg `/i[fire-ring]`)
- `{number}` embeds an externally-provided parameter (eg `{0}`). The identity of these parameters is typically enumerated in a comment above entries that use them
- `{number,plural,args}` uses an externally-provided parameter to choose text using the [ICU MessageFormat](https://unicode-org.github.io/icu/userguide/format_parse/messages/) standard
  - For example, `{0,plural,=0{It's exactly zero}=1{It's exactly one}other{It's #}}` checks parameter 0, checks if it's equal to 0 and prints "It's exactly zero" if so, otherwise checks if it's equal to 1 and prints "It's exactly one" if so, otherwise prints the parameter itself with `#`
  - ICU MessageFormat can do a lot more than just this, but this is the only part that I use

## Translation style guide

This game's battle mechanics make an effort to use extremely consistent language and formatting across all situations so that someone familiar with the game's terminology can interpret mechanics much more quickly

Please try to keep this consistency as best you can (and if you see a place where I broke my own rules, please let me know so I can correct it)

This game makes many references. Most references are notated in this file, but many skills/passives also share names with skills/passives from Pokémon and Shin Megami Tensei, which is not notated in every case because it's frequent and not very important

You don't have to preserve every reference in translation, since some may not make sense when translated

For items where the name is not a reference or otherwise lore-critical, it is not particularly important that the translation keeps the exact same meaning as long as it conveys the same vibe (For instance, "Thunder Spear" could easily be replaced with anything that just as strongly implies a strong Str-based Fulgur attack; just make it sound cool and intuitive)