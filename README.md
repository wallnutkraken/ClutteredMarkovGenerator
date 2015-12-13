##How do I use this library?
It's pretty simple. Just feed your original lines using Markov.Feed(), and then use MarkovGenerator.Create() to get a Markov Chain sentence

Once you need to save the chain state, you can call Save(), which will save the chain information in the current directory. And when you need it again, you can use Load().
