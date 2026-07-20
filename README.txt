Task 1 — Research: Spanish Spot Power Exchange
1. What is the Spanish Spot Power Exchange?

The Spanish spot electricity market is operated by OMIE — Operador del Mercado Ibérico de Energía. OMIE operates the organized wholesale electricity markets for Spain and Portugal and acts as the nominated electricity market operator, or NEMO, for both countries.

The Spanish Day-Ahead market is not an isolated Spanish auction. It forms part of the European Single Day-Ahead Coupling, or SDAC. Electricity buyers and sellers submit bids for delivery on the following day. A common European algorithm matches supply and demand while considering available cross-border transmission capacity.

For each market period, the auction produces a single marginal clearing price for the Spanish bidding zone, expressed in EUR/MWh. It is not a separate electricity price for every generating company, as share prices are separate for each company on a stock exchange.

2. Where are official Day-Ahead results published?

The primary official source is the OMIE website.

The navigation path is:

Market Results → File Access → Day-Ahead Market → Prices → Day-Ahead market hourly prices in SpainThe public repository generally provides files for the latest six rolling years. OMIE directs users to its assistance service for older files.

OMIE also provides a visual Day-Ahead results page, but the downloadable file repository is more suitable for an automated application because it provides a consistent machine-readable format.

The ENTSO-E Transparency Platform is an additional official European source for Day-Ahead bidding-zone prices. However, for this assessment, OMIE should be treated as the primary source because OMIE operates and publishes the Spanish market auction results directly. ENTSO-E can be used as a validation or cross-checking source.

3. Which exact dataset/file should be used?

For a Spanish Day-Ahead spot-price application, the correct OMIE dataset is:

MARGINALPDBC

The official filename pattern is:

marginalpdbc_YYYYMMDD.v

4. How is the file structured?

The official schema is:

Year;Month;Day;Period;Portuguese marginal priceT;Spanish marginal price;

7. Key assumptions and limitations
Market price, not final customer price

MarginalES is the wholesale Day-Ahead auction price. It does not include retail margins, network tariffs, taxes, balancing costs or other components of an electricity bill.

Market result, not final physical dispatch

After the Day-Ahead auction, the system operator checks whether the resulting programme is technically feasible. Technical constraints and later markets can alter the final physical generation schedule. Therefore, MARGINALPDBC represents the Day-Ahead market-clearing price, not the final real-time dispatch.

Spain and Portugal are separate bidding zones

The two price columns must be retained or deliberately selected. For Spanish analysis, use MarginalES; do not select the first price column merely because it appears first

