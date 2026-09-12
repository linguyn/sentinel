
## STRUCTURE

                                                                  Unity (client)
                                                                        |
                                                                        |
                                                                Game State/Observation
                                                                        |
                                                                        | HTTP request
                                                                        | body: JSON  
                                                                  FastAPI Backend 
                                                                        |
                                                                        | 
                                                                AI Agent - test logic
                                                                        |
                                                                        |
                                                                      Action 
                                                                        |
                                                                        | HTTP response
                                                                        | body: JSON
                                                                      Unity
                                                                        |
                                                                        |
                                                                    Execute Actions 
                                                                        | 
                                                                        |
                                                                      Logs
                                                                        | 
                                                                        | 
                                                                    Database 
                                                                        | 
                                                                        | 
                                                                    Bug Detector 
                                                                        | 
                                                                        | 
                                                                    Bug Report 

**Stage 1** — Backend boundary.
**Stage 2** — Unity ↔ Backend.
**Stage 3** — Test Environment.
**Stage 4** — Deterministic Agent.
**Stage 5** — AI Agent.
**Stage 6** — Telemetry + Database.
**Stage 7** — Bug Detection.
**Stage 8** — AI Bug Analysis.
**Stage 9** — Generalize beyond Unity.