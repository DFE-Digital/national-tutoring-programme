import { Then } from "@badeball/cypress-cucumber-preprocessor";

Then("they will be taken to the 'Compare national tutoring options' page", () => {
    cy.location('pathname').should('eq', '/options');
});

Then("they will be taken to the 'Find a tuition partner' journey start page", () => {
    cy.location('pathname').should('eq', '/find-a-tuition-partner');
});

Then("they will be taken to the 'Which key stages' page", () => {
    cy.location('pathname').should('eq', '/find-a-tuition-partner/which-key-stages');
});