namespace BuberDinner.Contracts.MenuReviews;

public record PostMenuReviewRequest(
    int Rating,
    string Comment,
    string DinnerId);