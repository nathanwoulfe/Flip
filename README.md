## First, a quick history lesson

Cast your mind back, if you will, to the heady days of Umbraco 7. We had a fresh new backoffice, shiny and glorious.

In among the tools and features, we had that most powerful button - Change Document Type. As the name would suggest, this humble button allowed flipping content nodes from one document type to another.

Alas, when Umbraco 8 hit the scene, the Change Document Type button was lost to the annals of CMS history.

This was intentional, and sensible. Over time, Umbraco's property editors became increasingly complex, often storing large JSON data structures rather than primitive values. 

That change made changing document types more difficult and error prone (although, in the wrong hands, the feature always had the potential to wreak havoc on a website).

However, deep in the bowels of Umbraco, the magic required to convert document types lay dormant. Like Smaug on his mountain of ill-gotten loot.

Flip is here to wake the dragon.

## What's the deal?

Flip adds an action to the context menu to allow changing the current document to a new type.

It is not foolproof, but will provide a list of valid document types for the current location and suggest an appropriate template. When calculating valid types, Flip considers the permitted child types of the current node and its parent, so many not always allow conversion.

Once a new type is selected, users can map properties from the old/current type to the new. Unmapped properties are discarded.

Flip offers two mapping modes:

 - Data type: allow mapping values to properties using the same data type (ie Textstring, of type Umbraco.Textbox)
 - Property editor: allow mapping values to properties using the same property editor (ie any instance of Umbraco.Textbox)

The former results in tighter mapping rules (ie different Grid editors can only map to the exact same data type).

## Buyer, beware!

Is this a super safe, bulletproof tool? No. 

Does it work for most cases? Yes, probably.

For any complex data - Block Lists, Nested Content - it's probably not a great idea, but will still allow in-situ conversion of document types, which is still an easier process for editors compared to creating a new node, moving child nodes, deleting the old node, moving the new node and children.

As property mapping is opt-in, it's completely viable to map simple values, ignore Block List, and re-build the list on the new type.

To re-iterate: this is an experimental package. Experiment at your own risk.

## Supported versions

Flip will play nice with Umbraco 8 and above.

## Installation

`Install-Package Flip.Umbraco` or `dotnet add package Flip.Umbraco`

## Credits

Icon: flip by Joshua Weber from <a href="https://thenounproject.com/browse/icons/term/flip/" target="_blank" title="flip Icons">Noun Project</a>
