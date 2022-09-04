# Inventory CLI

Save your products in a list using your CLI.

##### Command tree of all (sub-) commands and parameters:
```
> inv [command | alias] [subcommand | alias] [parameter]

[ item | -i ]
+--[ add | -a ]
|  +--[ ean | -e ]
|  +--[ quantity | -q ]
|  +--[ description | -desc ]
|  +--[ unit | -u ]
+--[ edit | -e ]
|  +--[ ean | -e ]
|  +--[ quantity | -q ]
|  +--[ description | -desc ]
|  +--[ unit | -u ]
+--[ delete | -d ]
|  +--[ ean ]
+--[ increase | -inc ]
|  +--[ ean ]
+--[ decrease | -dec ]
|  +--[ ean ]
+--[ list | -l ]

[ tag | -t ]
+--[ add | -a ]
|  +--[ ean | -e ]
|  +--[ name | -n ]
+--[ remove | -r ]
|  +--[ ean | -e ]
|  +--[ name | -n ]
+--[ list | -l ]
   +--[ tag ]

[ settings | -s ]
+--[ setpath ]
|  +--[ path ]
+--[ getpath ]

[ version | -v ]

[ help | -h ]
```

Configure your `inventory.csv` path with:
`> inv settings setpath <PATH>`
