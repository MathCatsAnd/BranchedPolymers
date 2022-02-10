# BranchedPolymers
2D Branched Polymer Generation: Uniformly random generation

This repository is seeded with code originally written in 2015. As part of an effort to build a portfolio, I have copied it here as-is with the intent to clean it up.

A 2D branched polymer is a connected set of non-overlapping circles. A tree is formed with nodes at the center of each circle and edge drawn between tangentially touching circles. A measure space is formed for the set of all branched polymers with n circles as follows:

Given circles labeled 1 through n, with each circle c(i) have some radius r(i), consider the center of c(1) to be the root node of the associated tree. A branched polymer can be identified first by its tree then by the angle of attachment of each child node to its parent. Within this measure space, branched polymers can be generated uniformly at random and this code does exactly that.

As it is from 2015, the code is able to generate images and animations. The math which accomplishes the uniformly random generation does so by 'growing' a branched polymer one node at a time, constantly shifting the branched polymer throughout the process as it makes room for added circles muscling their way in.

This readme will be updated as I beautify the code and interface.

The algorithm for generating branched polymers uniformly at random was obtained from a 2007 paper by Richard Kenyon and Peter Winkler. "Branched Polymers" can be found in the mathematics section of the arXiv.
arXiv:0709.2325v1 [math.PR]
https://arxiv.org/pdf/0709.2325v1.pdf
