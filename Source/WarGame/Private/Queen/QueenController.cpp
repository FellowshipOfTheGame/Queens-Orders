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

/*
void ABuildingSlot::Tick(float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);

	if (widget && widget->IsVisible()){

		APlayerController *controller = GetWorld()->GetFirstPlayerController();
		if (!controller)
			return;

		AQueenCamera *rtsCamera = Cast<AQueenCamera>(controller->GetPawn());
		if (!rtsCamera)
			return;

		// Resize
		float dist = FVector::Dist(rtsCamera->camera->GetComponentLocation(), GetActorLocation()) / 1100.0;
		widget->SetWorldScale3D(FVector(dist, dist, dist));
		// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("Scale: %f"), dist));

		// Look direction
		widget->SetRelativeRotation(FRotator(0, 90, 0));

		// This should be on Init() call
		widget->SetRelativeLocation(FVector(0, 0, 1800));
	}
}

void ABuildingSlot::ShowOptions()
{
	if (building){
		building->ShowOptions();
	}
	else if (widget)
	{
		widget->SetHiddenInGame(false);
		widget->SetVisibility(true);
		widget->Activate();
	}

	// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("SHOW: %p"), widget));
}

void ABuildingSlot::HideOptions()
{
	if (building){
		building->HideOptions();
	}

	if (widget){
		widget->SetHiddenInGame(true);
		widget->SetVisibility(false);
		widget->Deactivate();
	}

	// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("HIDE: %p"), widget));
}*/