# Project Plans

This folder contains the technical implementation plans, design notes, and task breakdowns for the Apartment Management System.

---

## Active & Next Up

- **[`validation-layers.md`](validation-layers.md)**: Plan for three-layer validation (Database CHECK constraints, DTO validation, and service-level business rules) based on feedback from Eng. Mohammed Tala'at.

---

## Completed & Archived Plans

Previous feature plans that have already been implemented and merged into main:

- **[`archive/clear-error-messages.md`](archive/clear-error-messages.md)**: Standardized error responses using `ApiErrorMapper` to return clean 400/409 errors instead of raw SQL crashes (Done in [v0.7.1](https://github.com/Medo-SM/ApartmentManagementSystem/releases/tag/v0.7.1)).
- **[`archive/field-validation.md`](archive/field-validation.md)**: Added Data Annotations across all 7 DTOs for API input validation (Done in [v0.7.0](https://github.com/Medo-SM/ApartmentManagementSystem/releases/tag/v0.7.0)).
- **[`archive/automated-tests.md`](archive/automated-tests.md)**: Set up the unit test project (`src/Tests`) using xUnit + Moq with 73 tests for all services (Done in [v0.7.0](https://github.com/Medo-SM/ApartmentManagementSystem/releases/tag/v0.7.0)).
- **[`archive/github-issues.md`](archive/github-issues.md)**: Draft issue templates used to set up the validation and testing tasks on GitHub.
