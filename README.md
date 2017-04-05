# full-text-search
Console based indexing and search system for text files using C#. 

### Installation instructions

1. Clone the repository
  `git clone git@github.com:uvinod/full-text-search`
 
2. Run the project using Visual Studio

### Usage

#### Indexing
To Generate index for a particular text file, run

```
index <path to text file>
```

```
example:

index C:\Users\windows\Documents\Sculpt\TextSearch\text.txt
```
This generates an index file in JSON format, and writes the words and it's occurances count with file name.

#### Searching
To perform search against indexed words, run

```
search word1 word2 word3
```

```
example:

search apple sky

Searching for apple...

===
Found in:
sample.txt(2)
test.txt(1)

Searching for sky...

No Matches Found
```

Results are sorted based on number of occurences of the word in the file.
