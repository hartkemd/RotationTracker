# RotationTrackerApp
This application tracks employee rotations, where employees take turns on weekly, biweekly, monthly, or bimonthly duties.

## Examples
### On-call Rotation:
1. Mark, Tim, Sue, and Bob are in an on-call rotation in that order
2. The on-call rotation moves to the next person every week on a Sunday
3. Mark takes his turn and Tim is up
4. Mark goes back to the end of the list
5. The application will show the order of the employees in this rotation
6. Administrators can advance the rotation, to change the employee who is currently up
7. The application will advance the rotation automatically every week on Sunday, or when the app is opened, if after the date/time the rotation advances

### Monthly Meeting Rotation:
1. Tim, Sue, Bob, and Mark are in a meeting rotation in that order
2. There is a meeting once a month
3. Tim takes his turn and Sue is up
4. Tim goes back to the end of the list
5. The application will show the order of the employees in this rotation
6. Administrators can advance the rotation, to change the employee who is currently up
7. The application will advance the rotation automatically every month, after the date/time the rotation advances has passed

## Installation Instructions
Download and extract the .zip file from the Releases section to the desired installation directory.

## How to Use
### Setup
1. After extracting the .zip file to your installation directory, download and install the DB Browser for SQLite from their website.
2. Open the DB Browser for SQLite.
3. Open the RotationDB.db file from the RotationTracker installation directory.
4. Click the Browse Data tab.
5. Select the Admins table, if it's not already selected.
6. Insert a new record; for the UserName, enter your Windows username.
7. Insert new records for all other administrators of the app.

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
### Current Features:
* Administrators can add and remove employees.
* Administrators can add and remove rotations.
* Administrators can advance rotations.
* Administrators can edit rotations.
* Employees can view the rotations the administrator has set up, including who is up and the next date/time the rotation advances.
* Rotations can advance every week, every other week, every month, or every other month.
* Rotations automatically advance, after the date/time they advance has passed. The app checks if the rotations need to advance every time the app is opened and every 15 minutes, while the app is running.
* Data is stored to a SQLite database that is kept with the app.

### Future Features/Ideas:
* Maybe something that will notify the next employee in the list that their turn is coming soon?
