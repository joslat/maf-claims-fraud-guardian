# TODO - Claims Fraud Guardian Workflows

## 📋 High-Level Summary

### 🔴 High Priority (Pending)
1. **Update XML Documentation** - Polish inline documentation comments for agents and executors
2. **Improve Documentation** - Create comprehensive docs for workflows, agents, and patterns
3. **MCP Server Toggle** - Implement configuration-based toggle between direct calls and MCP server (⚠️ Blocked: Docker)

### 🟡 Medium Priority (Pending)
4. **Add Test Layer** - Create comprehensive test suite for tools, agents, workflows, and patterns

### 🟢 Low Priority / Future
5. **State Updates in Fan-Out** - Enable state access in fan-out executors (⚠️ Blocked: Framework #2678)
6. **DevUI Enhancements** - Improve visualization for Claims Intake and fan-out/fan-in patterns (⚠️ Blocked: Framework #2691)
7. **Additional Documentation** - ADRs, deployment guides, performance tuning, troubleshooting
8. **Backlog Ideas** - Multi-language support, voice input, photo analysis, real DB integration, etc.

---

## 📊 Overview

This document tracks planned improvements, technical debt, and future enhancements for the Claims Fraud Guardian workflow system.

---

## 🔴 High Priority

### 1. Update XML Documentation

**Status**: 🟡 Planned  
**Priority**: High  
**Effort**: Low

Polish inline documentation comments for agents and executors to improve code clarity and maintainability.

#### Action Items

- [ ] Update XML documentation comments for all agents
- [ ] Update XML documentation comments for all executors
- [ ] Ensure consistency in documentation format
- [ ] Update GitHub repository documentation

---

### 2. Improve Documentation to Expose Clearer Workflow and Agent Responsibilities

**Status**: 🟡 Planned  
**Priority**: High  
**Effort**: Medium

Create comprehensive documentation for workflows, agents, and patterns to improve understanding and maintainability.

#### Proposed Documentation Structure

```
docs/
??? workflows/
?   ??? claims-intake-workflow.md
?   ?   ??? Overview
?   ?   ??? Architecture Diagram
?   ?   ??? Executor Responsibilities
?   ?   ??? Agent Responsibilities
?   ?   ??? State Management
?   ?   ??? Error Handling
?   ??? fraud-detection-workflow.md
?       ??? Overview
?       ??? Architecture Diagram (Fan-Out/Fan-In)
?       ??? Parallel Analysis Agents
?       ??? Aggregation Strategy
?       ??? State Management Rules
?       ??? Decision Logic
??? agents/
?   ??? customer-intake-agent.md
?   ??? validation-enrichment-agent.md
?   ??? online-marketplace-intelligence-agent.md
?   ??? customer-history-analysis-agent.md
?   ??? transaction-risk-analysis-agent.md
??? patterns/
    ??? fan-out-fan-in-pattern.md
    ??? turn-token-pattern.md
    ??? state-management-best-practices.md
    ??? devui-executor-patterns.md
```

#### Documentation Templates

**Workflow Template:**
```markdown
# [Workflow Name]

## Purpose
[What business problem does this solve?]

## Architecture
[Mermaid diagram showing executor flow]

## Executors
| Executor | Input | Output | Responsibility | State Access |
|----------|-------|--------|----------------|--------------|
| ... | ... | ... | ... | ? / ? |

## Agents
| Agent | Role | Tools | Output Format |
|-------|------|-------|---------------|
| ... | ... | ... | ... |

## State Management
[How state flows through the workflow]

## Error Handling
[How errors are handled at each stage]

## Testing
[How to test this workflow]
```

**Agent Template:**
```markdown
# [Agent Name]

## Role
[What is this agent's primary responsibility?]

## Input
[What data does it receive?]

## Output
[What does it produce?]

## Tools Available
- Tool 1: [Purpose]
- Tool 2: [Purpose]

## Decision Logic
[How does it make decisions?]

## Example Interaction
[Real example of input/output]

## Error Cases
[How it handles errors]
```

#### Action Items

- [ ] Create `docs/workflows/` folder with detailed workflow documentation
- [ ] Create `docs/agents/` folder with individual agent documentation
- [ ] Create `docs/patterns/` folder with reusable pattern documentation
- [ ] Add Mermaid sequence diagrams for each workflow
- [ ] Document state management rules clearly (especially fan-out restrictions)
- [ ] Add decision trees for routing logic
- [ ] Create "Quick Start" guides for each workflow
- [ ] Add troubleshooting section with common issues

---

### 3. Use the MCP Server as a Toggle

**Status**: ⚠️ Blocked (Docker installation issue)  
**Priority**: High  
**Effort**: Medium  
**Blocked By**: Docker Desktop installation constraints

Implement configuration-based toggle between direct tool calls and MCP server calls for development flexibility and deployment options.

#### Action Items

- [ ] Install Docker Desktop (prerequisite)
- [ ] Deploy ClaimsCoreMcp as Docker container
- [ ] Test MCP server with SSE transport
- [ ] Create `IClaimsToolProvider` abstraction
- [ ] Implement `DirectClaimsToolProvider`
- [ ] Implement `McpClaimsToolProvider`
- [ ] Add configuration support in `appsettings.json`
- [ ] Update workflows to use `IClaimsToolProvider`
- [ ] Add integration tests for both modes
- [ ] Document toggle configuration in README

---

## 🟡 Medium Priority

### 4. Add Test Layer for Workflows

**Status**: 🟡 Planned  
**Priority**: Medium  
**Effort**: High

Create comprehensive test suite for tools, agents, workflows, and patterns to ensure reliability and maintainability.

#### Test Coverage Needed

- **Tool Usage Tests**: Verify tools are called with correct parameters
- **Agent Outcome Tests**: Verify agents produce correct output
- **Workflow Integration Tests**: End-to-end workflow execution
- **Fan-Out/Fan-In Pattern Tests**: Verify parallel execution and aggregation

#### Action Items

- [ ] Create `ClaimsCore.Workflows.Tests` project
- [ ] Implement tool usage tests (all ClaimsTools methods)
- [ ] Implement agent outcome tests (each agent)
- [ ] Implement workflow integration tests (full scenarios)
- [ ] Implement fan-out/fan-in pattern tests
- [ ] Create test utilities (simulators, mocks, builders)
- [ ] Add CI/CD integration (run tests on every commit)
- [ ] Create test coverage report (target: >80%)
- [ ] Document testing strategy in `docs/testing/`

---

## 🟢 Future Enhancements

### 5. Implement State Updates on Each Executor

**Status**: ⚠️ Blocked by Framework Issue  
**Priority**: Medium  
**Effort**: Low (once unblocked)  
**Blocked By**: [microsoft/agent-framework#2678](https://github.com/microsoft/agent-framework/issues/2678)

Enable state access in fan-out executors for better persistence, observability, and resumability.

**Current Limitation**: Fan-out executors cannot access workflow state without breaking the pattern.

**Desired**: Fan-out executors can read/write state for immediate persistence and better observability.

#### Action Items

- [ ] Monitor [agent-framework#2678](https://github.com/microsoft/agent-framework/issues/2678) for resolution
- [ ] Test state access in fan-out executors after framework update
- [ ] Refactor fan-out executors to use state once unblocked
- [ ] Remove message-passing workaround if no longer needed
- [ ] Update documentation with new pattern

---

### 6. Implement DevUI Visualization for Claims & Fraud Detection Workflows

**Status**: ⚠️ Blocked by Framework Issue  
**Priority**: Low (nice-to-have)  
**Effort**: Medium (once unblocked)  
**Blocked By**: [microsoft/agent-framework#2691](https://github.com/microsoft/agent-framework/issues/2691)

Improve DevUI visualization for Claims Intake workflow and fan-out/fan-in patterns in fraud detection.

**Current Status**:
- ✅ ClaimsFraudDetectionWorkflow_DevUI works with Chat Protocol
- ❌ ClaimsIntakeWorkflow lacks DevUI support (uses console input)
- ⚠️ Fan-out/fan-in visualization shows flat execution (no parallel indication)

**Desired**:
- Fan-out/fan-in visual representation
- State inspection at each workflow step
- Interactive Claims Intake workflow for DevUI

#### Action Items

- [ ] Monitor [agent-framework#2691](https://github.com/microsoft/agent-framework/issues/2691) for resolution
- [ ] Create `ClaimsIntakeWorkflow_DevUI.cs` following Chat Protocol pattern
- [ ] Test DevUI with refactored Claims Intake workflow
- [ ] Enhance fraud detection DevUI visualization (once framework supports it)
- [ ] Add state inspection UI (if framework provides hooks)
- [ ] Document DevUI patterns in `docs/patterns/devui-executor-patterns.md`
- [ ] Create screenshots/videos of DevUI workflows for documentation

---

## 📚 Documentation

### 7. Additional Documentation Needs

**Status**: 🟡 Planned  
**Priority**: Low  
**Effort**: Medium

Create comprehensive documentation including ADRs, deployment guides, performance tuning, and troubleshooting.

#### Action Items

- [ ] Create `docs/adr/` folder with architecture decision records
- [ ] Create `docs/deployment/` folder with deployment guides
- [ ] Create `docs/performance/` folder with tuning guides
- [ ] Create `docs/troubleshooting/` guide
- [ ] Create `CONTRIBUTING.md` in repository root
- [ ] Add diagrams (Mermaid, PlantUML, draw.io)
- [ ] Create video walkthrough of workflows
- [ ] Add link to documentation in README

---

## 💡 Backlog

### 8. Other Future Ideas

**Status**: 💭 Ideas  
**Priority**: Low  
**Effort**: TBD

Various enhancement ideas for future consideration including multi-language support, voice input, photo analysis, real database integration, and more.

#### Categories

- **Workflow Enhancements**: Multi-language, voice input, document upload, photo analysis, geolocation
- **Agent Enhancements**: Sentiment analysis, legal compliance, payout estimation, settlement negotiation
- **Tool Enhancements**: Real database, external APIs, credit bureau, police reports
- **DevUI Enhancements**: Real-time collaboration, approval workflows, audit trail, dashboard
- **Performance & Scalability**: Persistence, distributed execution, load balancing, caching
- **Security & Compliance**: GDPR, audit logging, RBAC, encryption

---

## ?? Priority Matrix

| Task | Priority | Effort | Impact | Status |
|------|----------|--------|--------|--------|
| 1. Update Names | High | Medium | High | ?? Planned |
| 2. Improve Documentation | High | Medium | High | ?? Planned |
| 3. MCP Toggle | High | Medium | High | ?? Blocked (Docker) |
| 4. Add Test Layer | Medium | High | High | ?? Planned |
| 5. State in Fan-Out | Medium | Low | Medium | ?? Blocked (Framework) |
| 6. DevUI Visualization | Low | Medium | Medium | ?? Blocked (Framework) |
| 7. Additional Docs | Low | Medium | Medium | ?? Planned |
| 8. Backlog Ideas | Low | TBD | Low-High | ?? Ideas |

---

## ?? Next Steps

### Immediate (Next Sprint)

1. ? Create this TODO document
2. ?? Rename key files and folders (Task 1)
3. ?? Create workflow documentation (Task 2)
4. ?? Start tool usage tests (Task 4.1)

### Short Term (Next Month)

1. ?? Complete renaming and documentation
2. ?? Implement MCP toggle (if Docker available)
3. ?? Create comprehensive test suite
4. ?? Deploy ClaimsCoreMcp as Docker container

### Long Term (Next Quarter)

1. ?? Monitor framework issues (#2678, #2691)
2. ?? Implement state access in fan-out (when unblocked)
3. ?? Create DevUI version of Demo11 (when unblocked)
4. ?? Evaluate backlog ideas for implementation

---

## 📞 Contact

For questions or discussions about this TODO list:

- **Repository**: [maf-claims-fraud-guardian](https://github.com/joslat/maf-claims-fraud-guardian)
- **Issues**: Use GitHub Issues for tracking
- **Discussions**: Use GitHub Discussions for questions

---

**Last Updated**: January 29, 2025  
**Next Review**: February 15, 2025

---

## ✅ DONE

### 1. Workflow and File Renaming (Completed January 29, 2025)

Successfully renamed all workflow files and classes to production-ready names:

#### Files Renamed
- ✅ `Demo11_ClaimsWorkflow_Integrated.cs` → `ClaimsIntakeWorkflow.cs`
- ✅ `Demo12_ClaimsFraudDetection_Integrated.cs` → `ClaimsFraudDetectionWorkflow.cs`
- ✅ `Demo12_ClaimsFraudDetection_DevUI.cs` → `ClaimsFraudDetectionWorkflow_DevUI.cs`
- ✅ `ClaimsIntegrated` folder → `Workflows` folder

#### Classes Renamed
- ✅ `Demo11_ClaimsWorkflow_Integrated` → `ClaimsIntakeWorkflow`
- ✅ `Demo12_ClaimsFraudDetection_Integrated` → `ClaimsFraudDetectionWorkflow`
- ✅ `Demo12_ClaimsFraudDetection_DevUI` → `ClaimsFraudDetectionWorkflow_DevUI`

#### Documentation Updated
- ✅ Updated `Program.cs` menu entries and method calls
- ✅ Updated all README files with new workflow names
- ✅ Cleaned up user-facing text (removed "Demo" and "Integrated" references)
- ✅ Fixed emoji rendering issues in console output
- ✅ Updated TODO.md status tracking

#### Impact
- All workflows now have professional, production-ready names
- Consistent naming across production workflows and DevUI variants
- Clear separation between Workflows (production) and DevUI (visualization)
- Build successful with all changes

