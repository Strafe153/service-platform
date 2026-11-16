---
name: design
description: Guidelines for creating and modifying application UI design and visual identity, including layouts, component styling, typography, color schemes, themes, and responsive design. Use when implementing or modifying frontend appearance.
---

# Application Design and Visual Identity

When creating or modifying application design and visual identity, follow these guidelines.

## Design Guidelines

### Existing Architecture and Styling

Before making changes:

1. Inspect the existing theming configuration and design system.
2. Identify the styling solutions and libraries currently in use.
3. Review existing reusable components and CSS styles.
4. Prefer extending existing styles and components over introducing new styling approaches.
5. Maintain consistency with the existing application design unless explicitly asked to redesign it.

### Visual Design

1. Prioritize usability, readability, accessibility, and visual consistency.
2. Prefer clean, modern layouts over excessive decoration.
3. Maintain consistent spacing, typography, and color schemes.
4. Use the existing theme and design tokens where available.
5. Avoid unnecessary gradients, shadows, animations, and decorative elements.
6. Ensure responsive behavior across relevant screen sizes.

### Code Quality

1. Follow existing React and TypeScript conventions.
2. Adhere to the project's ESLint and Prettier configuration.
3. Prefer reusable components and maintainable styling.
4. Avoid introducing unnecessary dependencies.

## Forbidden Actions

- **Business Logic:** Do not modify business logic, API integrations, data models, validation rules, or application state management.
- **Component Behavior:** Preserve existing functionality and component behavior unless explicitly required by the requested design changes.
- **Dependencies:** Do not introduce new dependencies solely for styling when existing project tools are sufficient.
