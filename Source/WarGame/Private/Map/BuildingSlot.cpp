// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"

#include "BuildingSlot.h"

#include "MatchGameModeBase.h"
#include "QueenCamera.h"

ABuildingSlot::ABuildingSlot(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{

	PrimaryActorTick.bStartWithTickEnabled = true;
	PrimaryActorTick.bCanEverTick = true;

	building = nullptr;

}

bool ABuildingSlot::OnBuildOnSlot(EBuildingTypes type)
{
	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("OnBuildOnSlot: %d"), (int)type));

	// Already has a building?
	if (building){
		return false;
	}

	// Verify if this slot accepts the building type
	bool hasType = false;
	for (EBuildingTypes t : buildingTypes)
	{
		if (t == type)
		{
			hasType = true;
			break;
		}
	}

	if (!hasType)
		return false;

	// Spawn the correct building object	
	AMatchGameModeBase* gm = Cast<AMatchGameModeBase>(GetWorld()->GetAuthGameMode());
	// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("GM: %p"), gm));

	if (!gm)
		return false;

	// Create the building object
	switch (type){

		case EBuildingTypes::Ranger:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingRanger);
			break;

		case EBuildingTypes::Barracks: 
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingBarracks);
			break;

		case EBuildingTypes::Tower:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingTower);
			break;

		case EBuildingTypes::Gate:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingGate);
			break;

		case EBuildingTypes::Keep:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingKeep);
			break;

		case EBuildingTypes::GoldMine:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingGoldMine);
			break;

		case EBuildingTypes::StoneMine:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingStoneMine);
			break;

		default:
			return false;

	}

	building->SetActorLocation(GetActorLocation());

	return true;
}
