# RotationTrackerApp
This application tracks employee rotations, where employees take turns on weekly, monthly, or bi-monthly duties.

## Examples
### On-call Rotation:
1. Mark, Tim, Sue, and Bob are in an on-call rotation in that order
2. The on-call rotation moves to the next person every week on a Sunday
3. Mark takes his turn and Tim is up
4. Mark goes back to the end of the list
5. The application will show the order of the employees in this rotation
6. You can advance the rotation, to change the employee who is currently up, if you are an administrator
7. In the future, the application will be able to advance the rotation automatically every week on Sunday

### Monthly Meeting Rotation:
1. Tim, Sue, Bob, and Mark are in a meeting rotation in that order
2. There is a meeting once a month
3. Tim takes his turn and Sue is up
4. Tim goes back to the end of the list
5. The application will show the order of the employees in this rotation
6. You can advance the rotation, to change the employee who is currently up, if you are an administrator
7. In the future, the application will be able to advance the rotation automatically every month or after the employee has taken their turn

## Installation Instructions
None yet.

## How to Use
### To Edit the List of Employees:
1. Click the Edit button under the employees.
2. You can add or remove employees to/from the list here.

### To Add a Rotation:
1. Click the Add Rotation button.

### To Remove a Rotation:
1. Click the Remove Rotation button.
2. Select the rotation you'd like to remove.
3. Click the Remove button.

### To Edit a Rotation:
1. Click on the Edit button for the rotation you'd like to edit.
2. Modify the rotation as desired.
3. Click the Save and Close button.

## Roadmap
### Features That Need Work:
* Data access - I need to decide which type of data access is best for this app. SQLite or text files?
