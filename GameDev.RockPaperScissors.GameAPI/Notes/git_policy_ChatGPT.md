<h1>Q: </h1> How do I prevent code being merged into my main branch on GitHub? I want people to create their own branches and submit pull requests.

To ensure that code changes are reviewed and approved before being merged into the `main` branch in your GitHub repository, you can implement branch protection rules. These rules can enforce various policies, such as requiring pull requests for changes, needing approvals on pull requests, and passing status checks before merging. Here's how you can set up branch protection rules on GitHub:

### Setting Up Branch Protection Rules

1. **Navigate to Your Repository on GitHub**: Go to your GitHub repository where you want to enforce these rules.

2. **Go to Repository Settings**: Find and click on the "Settings" tab near the top of the repository page.

3. **Access Branches Settings**:
   - In the Settings menu, look for the "Branches" section in the sidebar on the left. This is where you can manage branch protection rules.

4. **Add a Branch Protection Rule**:
   - Click on the "Add rule" button to start setting up a new branch protection rule.
   - In the "Branch name pattern" field, enter `main` or the name of the branch you wish to protect. You can use wildcard characters to match multiple branches if needed.

5. **Configure the Rule**:
   - **Require Pull Request Reviews**: Check the box for "Require pull request reviews before merging". You can specify how many approvals are needed.
   - **Require Status Checks**: If you use continuous integration, you can enforce that all status checks pass before allowing a merge by enabling "Require status checks to pass before merging".
   - **Require Linear History**: Enabling "Require linear history" can help ensure that your commit history is clean and that all changes are made via pull request.
   - **Include Administrators**: Consider enabling "Include administrators" to apply these rules to everyone, including repository administrators.
   - **Restrict Who Can Push to Matching Branches**: If you want to further restrict who can push to the protected branch, you can specify users or teams under "Restrict who can push to matching branches". Note that this option is more restrictive and might be suitable for specific workflows.

6. **Save Changes**:
   - After configuring the settings to your liking, scroll down and click the "Create" or "Save changes" button to apply the branch protection rule.

### Enforcing Pull Requests
With these settings in place, changes to the `main` branch (or any branch you've protected) will require a pull request. Additionally, depending on your configuration, pull requests will need to meet the criteria you've set, like a certain number of reviews, passing status checks, etc., before they can be merged. This workflow encourages collaboration and code review, helping to maintain code quality and stability in your main branch.

Remember, these rules are flexible and can be adjusted to fit your project's workflow. You can always go back and modify the branch protection settings as your project evolves or as you find what works best for your team.