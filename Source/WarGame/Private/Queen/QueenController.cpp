// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "QueenController.h"

void AQueenController::BeginPlay()
{
	Super::BeginPlay();

	bShowMouseCursor = true;
	bEnableClickEvents = true;
	bEnableMouseOverEvents = true;
}

void AQueenController::setSelectedBuildSlot(ABuildingSlot *slot)
{
	if (currentBuildingSlot == slot)
		return;

	// Destroy current UI
	if (currentUI){
		currentUI->Destroy();
	}

	currentBuildingSlot = slot;

	// Show options for selected object
	if (currentBuildingSlot)
	{
		/* Check whether we need to show the UI for the Building or the BuildingSlot */
		if (currentBuildingSlot->building)
		{
			currentUI = OpenUIForBuilding(currentBuildingSlot->building);
		}
		else
		{
			currentUI = OpenUIForBuildingSlot(currentBuildingSlot);
		}
	}
}

ABuildingSlot* AQueenController::getSelectedBuildSlot() const
{
	return currentBuildingSlot;
}