~~~
Author:	Devin Fink
Partner: None
Start Date: 1/17/23
Course: CS 3500, University of Utah School of Computing
GitHub ID: deimos-5
Repo: https://github.com/uofu-cs3500-spring23/spreadsheet-deimos-5.git
Commit Date: 2/9/23 11:43PM
Solution: Spreadsheet
Copyright: CS 3500 and Devin Fink - This work may not be copied for academic coursework
~~~

# Overview of Spreadsheet Functionality
This is the overall solution for a spreadsheet application.
This spreadsheet program is currently able to take in any mathematical
formula and compute the answer. These formulas can have 
spreadsheet variables as part of them. The formulas are now 
more efficient and stored in immutable objects. Their syntax
is also checked during creation as opposed to 
processing. It can also keep track of 
cell dependencies. Cells are supported, with contents and
dependencies properly calculated for each cell and formula. 
Cells are fully functional, are evaluated as they change, and can be saved 
and retrieved from XML files.
Future extensions will feature a gui.

#Time Expenditures:

 Hours Expected/ Worked		Assignment											Note:
	5 / 8					Assignment 1: Formula Evaluator						Spent a large amount of time getting everything set up
	6 / 8					Assignment 2: Dependency Graph						Created a project-breaking bug through an incorrect file deletion.
	10 / 12					Assignment 3: Refactoring the FormulaEvaluator		Spent a large chunk of time understanding the instructions
	8 / 5					Assignment 4: Onward to a Spreadhseet				Pretty quick project. First session was primarily processing documentation
	10 / 12					Assignment 5: A Complete Spreadsheet Model			Very time expensive project that took a lot of information processing.

	Overall, I feel like my estimations could have been better across the board. I feel like I most often estimated low, because after a first pass through 
	the requirements it seems like the requirements are less than they actually are. Overall I feel as if my time management has been very effective, with a 
	good block of time at the begininng just working through the API, followed by a couple of straight coding sessions usually done in the CADE lab to reduce
	amount of time struggling on things I don't understand. Then usually I have 1 session dedicated to testing, which worked well for me.

Good Software Practices: 

1. The first software practice I'd like to highlight is a well named method that fulfills a specific functon. 
The "CheckName" method effectively bundles a null check and a regex into a small function that checks the validity 
of a name. Because this check happens so often through the spreadsheet, I thought it would be effective to put in 
a helper method to simplify it.

2. The second software practice I'd talk to talk about is code re-use of previous projects. Obviously these 
assignments have been designed to help us effectively implement the later stages of the spreadsheet, but I felt 
as though I utilized the dependency graph well in my implementation of the spreadsheet graph.

3. I'd further like to talk about my testing strategy. For the spreadsheet class in particular I mapped 
out all of my tests before I began the project to help guide me as I worked on the code. Although not all of the 
tests stayed the same as I discovered different things about the API, it helped me greatly in the initial starting
of the assignment and made the final testing at the end much easier.