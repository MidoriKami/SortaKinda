# SortaKinda
SortaKinda is a XivLauncher/Dalamud plugin.

SortaKinda is an inventory management plugin, worry no more about having to constantly sort your inventory, or lamenting how terrible /isort actually is.

With SortaKinda you define various sorting rules and item categories that fits your preferences, and then tell the plugin where you want those items to go, and in what order they should appear in.

Each `Sorting Rule` is made of a `Filter` and a `Ordering`.

![image](https://github.com/MidoriKami/SortaKinda/assets/9083275/0e5dc299-bd9c-41f3-b967-4613c617c8b9)

## Filters
Currently you can define two kinds of filters, "Name" or "Type".

Name filters use regular expressions to match items, `Prism` will match both `Clear Prism` and `Glamour Prism`.
You can use more advanced regex strings to filter items more specifically `Crached (Anth|Dend)` to match `Cracked Anthocluster` and `Cracked Dendrocluster`.

Type filters use the items type as the filter, all items display their item type below the tooltip icon.
SortaKinda provides a search box for quick lookups, and a complete table for enabling/disabling several item types at once.

![image](https://github.com/MidoriKami/SortaKinda/assets/9083275/8efd1ca3-55ee-44a1-8ff8-3d276b623c65)

![image](https://github.com/MidoriKami/SortaKinda/assets/9083275/7b314475-3f25-4cf8-bc33-0db678472488)

## Ordering
Currently you can select between one of five orderings, "Alphabetical", "Item Level", "Rarity", "Sell Price", or "Item Id"
It's worth noting, that "Rarity" refers to the items color, ie Pink, Green, Blue, Purple.

You can then choose to order them "Ascending", which puts the lower value first, or decending which puts the higher value first.
Ex. Ascending: `ilvl 200` will be before `ilvl 500`
Ex. Descending: `Butchers Knife` will be before `Augmented Cleaver`

Finally you can choose the order that SortaKinda fills the selected inventory slots.

Fill from Top, will place items starting with the first available inventory slot.
Fill from Bottom, will place items starting with the last available inventory slot.

![image](https://github.com/MidoriKami/SortaKinda/assets/9083275/96def8a1-8966-443e-9ea0-cff03f4ea8cb)




