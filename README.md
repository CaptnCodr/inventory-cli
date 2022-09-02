# Inventory CLI

Save your products in a list using your CLI.

##### Command tree of all (sub-) commands and parameters:
```
> inv [command] [subcommand] [parameter]

[ item ]
+--[ add ]
|  +--[ ean ]
|  +--[ quantity ]
|  +--[ description ]
|  +--[ unit ]
+--[ edit ]
|  +--[ ean ]
|  +--[ quantity ]
|  +--[ description ]
|  +--[ unit ]
+--[ delete ]
+--[ increase ]
+--[ decrease ]
+--[ list ]

[ tag ]
+--[ add ]
|  +--[ ean ]
|  +--[ name ]
+--[ remove ]
|  +--[ ean ]
|  +--[ name ]
+--[ list ]
   +--[ tag ]

[ settings ]
+--[ setpath ]
+--[ getpath ]

[ version ]

[ help ]
```

Configure your `inventory.csv` path with:
`> inv settings setpath <PATH>`
