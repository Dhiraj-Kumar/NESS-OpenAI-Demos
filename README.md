# SmartDesk — AI Powered IT Helpdesk System
### ASP.NET Web API + OpenAI Integration | Training Assignment

---

## Problem Statement

Organizations with large IT teams struggle to manage high volumes of support tickets efficiently. Support staff spend significant time manually reading, categorizing, and prioritizing tickets — leading to delayed responses, misrouted tickets, and inconsistent handling. Junior support agents often lack the knowledge to guide users through troubleshooting steps, and managers have no easy way to get a quick picture of weekly helpdesk performance.

**SmartDesk** solves this by integrating OpenAI into an ASP.NET Web API to automate ticket classification, provide real-time AI-assisted support chat, analyze user sentiment, and generate intelligent weekly reports — all through a clean, layered API.

---

## Objectives

By completing this assignment, you will be able to:

- Integrate the official OpenAI NuGet package into an ASP.NET Web API project
- Implement structured output using JSON schema to extract reliable data from AI responses
- Build a stateful multi-turn chat experience using session memory with IMemoryCache
- Stream long-form AI responses to clients using Server Sent Events (SSE)
- Apply a clean service layer architecture to AI-powered features
- Design RESTful endpoints that combine traditional API design with AI capabilities

---

## System Overview

SmartDesk exposes four functional modules, each targeting a real helpdesk pain point:

| Module | Feature | OpenAI Capability |
|--------|---------|-------------------|
| 1 | Ticket Classification | Structured Output |
| 2 | Support Chat Assistant | Chat + Streaming + Session Memory |
| 3 | Sentiment Analysis | Structured Output |
| 4 | Weekly Report Generation | Long-form Generation + Streaming |

---

## Module 1 — Ticket Classification

### Background
When a user submits a support ticket, a helpdesk coordinator must manually read it, determine the category, assign a priority, estimate resolution time, and route it to the correct team. This process is time-consuming and inconsistent across different coordinators.

### What It Does
This module accepts a raw support ticket and uses OpenAI to automatically classify it into a structured response. The AI reads the ticket title and description and returns a category, priority level, estimated resolution time, the team it should be assigned to, and a list of suggested first steps.

### Endpoints

---

#### `POST /api/ticket/classify`
Accepts a support ticket and returns a fully classified result.

**Accepts:**
- `title` — Short title of the issue
- `description` — Full description of the problem
- `submittedBy` — Email of the user who raised the ticket

**Returns:**
- `ticketId` — Auto generated unique ID for the ticket
- `submittedBy` — Email of the submitter
- `submittedAt` — Timestamp of submission
- `classification` — Object containing:
  - `category` — One of: Hardware, Software, Network, Security, Account, Other
  - `priority` — One of: Low, Medium, High, Critical
  - `estimatedResolutionHours` — Estimated time to resolve in hours
  - `summary` — One sentence summary of the issue
  - `assignTo` — One of: NetworkTeam, HardwareTeam, SoftwareTeam, SecurityTeam, AccountsTeam
  - `suggestedSteps` — List of at least 3 actionable resolution steps

---

## Module 2 — Support Chat Assistant

### Background
Users often need step-by-step guidance to resolve IT issues. A traditional chatbot with predefined flows cannot handle the variety of IT problems. Support agents are stretched thin and cannot always respond immediately. An AI assistant that remembers the conversation context and guides users intelligently can significantly reduce the load on human agents.

### What It Does
This module provides a conversational support assistant that maintains session memory across multiple turns. The AI introduces itself, asks clarifying questions, and guides the user through troubleshooting steps one at a time. Each session is stored in memory cache and expires after 30 minutes of inactivity. A streaming variant is also provided for real-time token-by-token responses.

### Endpoints

---

#### `POST /api/supportchat/message`
Sends a user message and receives a full AI response. Maintains conversation history across multiple calls using the same session ID.

**Accepts:**
- `sessionId` — Unique identifier for the conversation session
- `message` — The user's message

**Returns:**
- `sessionId` — Echo of the session ID
- `reply` — Full AI response text
- `turnCount` — Number of user turns in the current session

---

#### `POST /api/supportchat/stream`
Same as the message endpoint but streams the AI response token by token using Server Sent Events. Suitable for frontend applications that want to display the response progressively as it is generated.

**Accepts:**
- `sessionId` — Unique identifier for the conversation session
- `message` — The user's message

**Returns:**
- A stream of `data: <token>` events
- Ends with `data: [DONE]` to signal completion

---

## Module 3 — Sentiment Analysis

### Background
Not all support tickets carry the same emotional weight. A frustrated user whose issue has been unresolved for three days needs to be handled very differently from a user making a routine request. Without sentiment analysis, all tickets are treated equally and genuinely distressed users may not get timely attention.

### What It Does
This module analyzes the text of any customer message and returns a structured sentiment report. It detects whether the user is positive, neutral, negative, or frustrated, scores the confidence of that assessment, determines the urgency level, and flags whether the message requires immediate human intervention. This flag can be used by the system to auto-route high-risk tickets to a human agent.

### Endpoints

---

#### `POST /api/sentiment/analyze`
Analyzes the sentiment of a customer message and returns a structured report.

**Accepts:**
- `message` — The raw customer message to be analyzed

**Returns:**
- `sentiment` — One of: Positive, Neutral, Negative, Frustrated
- `confidence` — Float between 0 and 1 indicating confidence in the classification
- `urgency` — One of: Low, Medium, High
- `requiresHumanAgent` — Boolean flag indicating if immediate human intervention is needed
- `reason` — Brief explanation of why this sentiment classification was assigned

---

## Module 4 — Weekly Report Generation

### Background
IT managers need a clear picture of helpdesk performance at the end of each week. Compiling this manually from raw ticket data is tedious and the resulting reports are often dry and lacking in actionable insight. An AI-generated report can transform raw numbers into a professional narrative with highlighted trends and concrete recommendations.

### What It Does
This module accepts raw weekly ticket statistics and generates a structured, professional report in narrative form. The report includes an executive summary, key highlights, a breakdown of the top issues, and specific recommendations for the coming week. The response is streamed to the client so that the report appears progressively rather than after a long wait.

### Endpoints

---

#### `POST /api/report/weekly`
Accepts weekly ticket statistics and streams back a professionally formatted report.

**Accepts:**
- `totalTickets` — Total number of tickets received in the week
- `resolved` — Number of tickets resolved
- `pending` — Number of tickets still open
- `critical` — Number of critical tickets raised
- `topIssues` — List of the most frequently reported issue types
- `averageResolutionHours` — Average time taken to resolve a ticket in hours

**Returns:**
- A streamed `text/event-stream` response containing the report
- Report includes: Executive Summary, Key Highlights, Issues Breakdown, Recommendations
- Ends with `data: [DONE]` to signal completion

---

## Technical Requirements

- Framework: ASP.NET Web API (.NET 8)
- AI Provider: OpenAI via the official `OpenAI` NuGet package (v2.x)
- Architecture: Service layer pattern with interface-based dependency injection
- Memory: `IMemoryCache` for session-based conversation history
- Configuration: API key stored in environment variable
- Streaming: Server Sent Events (SSE) for Module 2 stream and Module 4

---
