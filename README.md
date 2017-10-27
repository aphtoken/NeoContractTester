<h1 align="center">NeoContractTester</h1>

<p align="center">
  A simple way to validate your AVM Contracts for the <b>NEO</b> Blockchain
  before spending a bunch of Neo Gas to deploy
</p>

!!! EXPERIMENTAL TOOL THAT IS USED TO VALIDATE CONTRACTS

1. Specify your AVM File Path that implements NEP-5.
    Cut and paste the file path from Explorer.

2. Add Storage variables. The easiest thing to test is to add the following
    variables:

    Variable 1 -
      Name: FROM
      Value: 1000
      Type: integer

    Variable 2 -
      Name: TO
      Value: 500
      Type: integer

3. Add a new Action. The two most useful actions that are supported right now
    are transfer and balanceOf

    Action 1 -
      Name: transfer
      Parameter Types: string, string, integer
      Return Types: integer

    Action 2 -
      Name: balanceOf
      Parameter Types: string,
      Return Types: integer

4. Run the actions

    Select transfer in the GUI
      Parameters - FROM, TO, 500. Hit the run button. Close the dialog.

    Select balanceOf in the GUI
      Parameters - TO. Hit the run button. Close the dialog. Results should be
      1000.  
