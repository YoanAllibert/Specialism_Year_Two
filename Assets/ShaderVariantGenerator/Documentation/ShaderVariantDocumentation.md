# **Shader Variant Generator Package: Specialism Brief 2**

## *Introduction*

Modern games require the generation of many variant versions of each custom shader you supply. Unity will automatically generate a superset of all the possible variations it might need for your project. This can waste gigabytes of memory, and slow both load times and build times. Shader keywords are one of the elements that cause the generation of shader variants.

Fortunately Unity includes a method to strip unwanted shaders from the build process. Unfortunately, you must manually create a file of all the specific shaders you want to preserve. This behaviour script automates the process by creating all possible combinations of provided lists.

## *ShaderVariantListGenerator*

This *script* can be added to any *GameObject*, and will deliver the output on *Start* method. The output is contained in a list sent to the console, each entry write a new line.

The input is a list (called **variants**) of lists (called **Keywords**).

### **Variants List**

This list represents the separate shaders that needs to be part of the variant list. Each of them contains two or more keywords to create variation.

### **Keywords List**

These lists represent the separate keywords contained in each shaders. The "null" keyword represent an empty string and will not be printed.

## *How it works*

To create a List of List, I created a *Serializable* class called *ListOfKeywords* that contains the list of keywords. In my *Generator* class, I have a list of the class *ListOfKeywords*. To organise the words in combination, we do the following in order:

* Calculate the numbers of output: That is simple the product of the number of keywords in each list.
* Calculate number of words in each next lists: This one is tricky. The way we are going to define variations, we need to know how many keywords are there **after** each list of keywords. In order to calculate this number we loop through the number of variations, and we work backward through the length of the nexts lists. Note: the last variant list has this number equal to one, not zero.
* Populate List of keywords: Using the information calculated before, we are now able to scroll through variants and create a list of repeating index per list. Here is an example to create conbinations for a list of *three* variants:

```
First List:   (indexOfOutput / list2*list3..) % nbOfWordInFirstList
Second List:  (indexOfOutput / list3..) % nbOfWordInSecondList
Third List:   (indexOfOutput) % nbOfWordInThridList
```

The `%`  sign indicated the *modulo operation*, which gives the remainder of a division. Let's apply this to Two lists containing respectively 2 and 3 keywords.

```
List 1: a, b
List 2: x, y, z
```

Total output: 2 x 3  = 6

Each list will be extended in 6 lines, with index of output *n*:

```
First List: (n / nbOfKeywordslist2) % nbOfWordInFirstList
0, 0, 0, 1, 1, 1

Second List: (n / 1) % nbOfWordInSecondList
0, 1, 2, 0, 1, 2
```

With these indexes saved, our final inputs are the corresponding keywords as if read from top to bottom, in line: `(0, 0), (0, 1), (0,2), (1, 0), (1, 1), (1,2)`.

Resulting in: `(ax), (ay), (az), (bx), (by), (bz)`
