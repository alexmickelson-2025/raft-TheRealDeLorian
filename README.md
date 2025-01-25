# Requirements

- [x] 1. when a leader receives a client command the leader sends the log entry in the next appendentries RPC to all nodes
- [x] 2. when a leader receives a command from the client, it is appended to its log
- [x] 3. when a node is new, its log is empty
- [x] 4. when a leader wins an election, it initializes the nextIndex for each follower to the index just after the last one in its log
- [x] 5. leaders maintain a "nextIndex" for each follower that is the index of the next log entry the leader will send to that follower
- [x] 6. Highest committed index from the leader is included in AppendEntries RPC's
- [ ] 7. When a follower learns that a log entry is committed, it applies the entry to its local state machine
- [ ] 8. when the leader has received a majority confirmation of a log, it commits it
- [ ] 9. the leader commits logs by incrementing its committed log index
- [ ] 10. given a follower receives an appendentries with log(s) it will add those entries to its personal log
- [ ] 11. a followers response to an appendentries includes the followers term number and log entry index
- [ ] 12. when a leader receives a majority responses from the clients after a log replication heartbeat, the leader sends a confirmation response to the client
- [ ] 13. given a leader node, when a log is committed, it applies it to its internal state machine
- [ ] 14. when a follower receives a valid heartbeat, it increases its commitIndex to match the commit index of the heartbeat
	1. reject the heartbeat if the previous log index / term number does not match your log
- [ ] 15. When sending an AppendEntries RPC, the leader includes the index and term of the entry in its log that immediately precedes the new entries
	1. If the follower does not find an entry in its log with the same index and term, then it refuses the new entries
		1. term must be same or newer
		2. if index is greater, it will be decreased by leader
		3. if index is less, we delete what we have
	2. if a follower rejects the AppendEntries RPC, the leader decrements nextIndex and retries the AppendEntries RPC
- [ ] 16. when a leader sends a heartbeat with a log, but does not receive responses from a majority of nodes, the entry is uncommitted
- [ ] 17. if a leader does not response from a follower, the leader continues to send the log entries in subsequent heartbeats  
- [ ] 18. if a leader cannot commit an entry, it does not send a response to the client
- [ ] 19. if a node receives an appendentries with a logs that are too far in the future from your local state, you should reject the appendentries
- [ ] 20. if a node receives and appendentries with a term and index that do not match, you will reject the appendentry until you find a matching log 