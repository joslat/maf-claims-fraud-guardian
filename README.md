# MAF Claims Fraud Guardian

A demo of the Microsoft Agent Framework for handling insurance claims and detecting fraud in a property theft scenario.

## What this demo shows

- A multi-agent workflow for filing and processing a property (bike theft) claim
- A user-facing “Claims Copilot” that collects all claim details
- Automatic routing to the correct workflow (property / bike theft)
- Parallel fraud checks (internal history, public property search, consistency checks)
- A final decision agent that explains approvals, rejections, or manual review

## Scenario

A customer reports a stolen bike.  
Agents collect the details, verify coverage, run fraud checks (including public marketplace search), and decide whether to approve, reject, or send the claim to a human adjuster — with transparent explanations.

## Tech

- Microsoft Agent Framework (MAF) workflows and agents
- MCP-style mocked backends for claims, contracts, and public property search
- Designed to plug into a Dev UI showing chat, workflow graph, and shared state
