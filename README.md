# The Name Game: UWP
"Leading scientists have proven, via science, that learning your coworkers names while starting a new job is useful." This program is a requested game to learn the names of coworkers on a team. It is written in C# in Visual Studio for the Univeral Windows Platform.

## Installation & Running
Windows (only):

Clone or download and open in Microsoft Visual Studio.
The minimum platform is Windows 10 (Fall Creators Update). An Internet connection is required to play.

## Game Features

Select any available options and hit "Play" to begin. Here are a few of your choices:
* **Normal Mode:** All names from the employee database are "in play." You're shown a name and asked to match it with one of five profile pictures. 
* **Matt Mode:** "Mat(t) Mode. Roughly 90% of our co-workers are named Mat(t)," and so in this mode only names beginning with Mat are in play.
* **M\* Mode:** Similar to Mat(t) mode, but this includes any names that start with M.
* **Team Mode:** Limits the names in play to profiles with actual job titles.
* **Reverse Mode:** Can be used in combination with the previous modes. The user is shown a picture and asked to match it with a name.
* **Matching Mode:** The user is shown five names and five pictures. Drag each name to its matching picture. -- _Not yet implemented_
* **Score:** The game keeps track of the number of correct and incorrect attempts. -- _Not yet implemented_

## Codebase Notes:
Business logic is mostly separated from the UI. There is also code to test the business logic.

