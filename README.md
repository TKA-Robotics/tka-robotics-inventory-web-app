# TKA Robotics Inventory System

You can view the live application [here](http://robotdbwebapp.azurewebsites.net).

## About

This application was developed by Joshua Atler (TKA class of 2017, member of MidKnight Madness) to create an organized system for tracking inventory and orders.

## Setup



## Models

The models folder contains the classes that define the tables for the database. The key tag identifies that ID is the key. The ID is not manually entered on the website because each new item is automatically assigned a new ID incrementally.

Some of the variables have a DisplayName tag when the variable is two words in order to add a space between the words on the website.

InventoryStatus and OrderStatus are enums, a special class that represents a group of constants, because they can only be certain predefined values.

TotalCost is a value derived from Quantity and UnitCost. By default, strings are optional and ints are required, so "int?" allows SuggestedQuantity and MinimumQuantity to be optional.

## Database

After creating, deleting, or editing any fields in any of the files in the models folder, you need to run the following commands in the command line to update the database in the cloud.

`dotnet ef migrations add changeName`

`dotnet ef database update`

Replace changeName with a short description of the change, but remember to keep it as one word without spaces.

## Views

The Views folder contains all of the CSHTML files that are displayed by the browser. CSHTML allows for C# code embedded within the HTML that standard HTML does not support.

The Shared folder contains _Layout.cshtml which is the primary file that all pages are based on. Within _Layout.cshtml is a \<partial\> tag that includes the _LoginPartial.cshtml file.

The @RenderBody function renders the main body of the page that may need to be edited. These files are included in the other folders within the Views folder.