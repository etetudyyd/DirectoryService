# DirectoryService

Directory Service stores the basic orgent guidelines - units, the positions of the Ilocation - and preemts the Unified CRUD interface for all internal services (HR, warehouse, orders, discounts of the Idr.). Thanks to this, the data is not duplicated, the company's astructure remains consistent.
Dozens of services work in the ecosystem: HR, warehouse, delivery, discounts IDR. All of them periodically need the same essence.
If each service will store these reference books separately, the names will inevitably disperse, the grunted departments will “come to life” at the faster database.
Directory Service is the "only source of truth."

The main tasks
Units (departments, branches)
Creation and storage of the list of all departments of the company.
Each unit can have a “parent” and “children” - the structure is built in the form of a tree.
Image
Locations (offices, buildings)
Storage of information about the places where the units are located.
One location can belong to different units, and vice versa.
Positions (position)
Description and storage of all posts of the company.
Each position can be available in several departments, and in one unit there can be many different positions.
Why is it necessary
To see the structure of the company - who is obeying to whom, where who works.
Quickly find in which office there are which departments or positions.
Store the history of changes and use this data for reports.
