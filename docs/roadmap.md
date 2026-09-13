---------------------------------------
STAGE 1
---------------------------------------

Data Flow: 

															  Unity Game State 
																	 | 
																	 | 
																	 | 
																	 |
															 extract information 
																	 | 
															         | 
																	 |
																	 |
																 Observation 
																	 | 
																	 | HTTP request 
																	 | body: JSON 
																	 |
															  FastAPI backend 
																	 | 
																	 | 
															         | 
																	 |
															   Action/Decision 
																	 | 
																	 | HTTP response 
																	 | body: JSON 
																	 | 
																   Unity
																	 |
																	 |
																	 |
																	 |
															 Execute the actions 
																

---------------------------------------
STAGE 2
---------------------------------------

Data flow: 

**Unity** ---> create Observation object ---> serialize to JSON ---> send HTTP ---> **FastAPI** ---> return action ---> **Unity receives responses**
